using FinApp.Helpers;
using FinApp.MiddleWare;
using FinApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace FinApp.MiddleWare
{
    public class StockManager : FinBase
    {
        private const string IndustryCollection = "industry";
        private YqlService _yqlService;
        public StockManager()
        {
            _yqlService = new YqlService();
        }

        public List<HistoryModel> GetHistory(string symbol, DateTime? startDate, DateTime? endDate)
        {
            using (var db = new DataStore<HistoryModel>("history"))
            {
                var history = db.GetCollection().Where(x => x.Symbol == symbol && x.Date >= startDate.Value && x.Date <= endDate.Value);
                if (!history.Any())
                {
                    if (startDate == null || endDate == null)
                    {
                        startDate = DateTime.Now.AddMonths(-11);
                        endDate = DateTime.Now;
                    }
                    var json = _yqlService.FetchHistory(symbol, startDate.Value, endDate.Value);
                    var jobj = JObject.Parse(json);
                    history = Helper.DeserializeJson<List<HistoryModel>>(JArray.Parse(jobj["query"]["results"]["quote"].ToString()));
                    history.ToList().ForEach(x => x.Symbol = symbol);
                }
                return history.ToList();
            }
        }


        /// <summary>
        /// Returns SMA for given parameters
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="averageOnDays"></param>
        /// <param name="model"></param>
        /// <returns>object[]{Average, Date}</returns>
        public string GetSimpleMovingAverage(DateTime startDate, int averageOnDays, List<HistoryModel> model)
        {
            var orderdedData = model.OrderBy(h => h.Date).Where(h => h.Date >= startDate);
            var lastDay = orderdedData.Last().Date;
            var firstDay = orderdedData.First().Date;
            var totalSize = Convert.ToInt32(orderdedData.Count() / averageOnDays);
            if ((orderdedData.Count() % averageOnDays) != 0) totalSize += 1;
            float[] average = new float[totalSize];
            string[] dateArray = new string[totalSize];
            int j = 0;
            List<object> data = new List<object>();
            for (int i = 0; i < orderdedData.Count();)
            {
                var avg = orderdedData.Skip(i).Take(averageOnDays).Average(h => h.Close);
                var date = orderdedData.Skip(i).Take(averageOnDays).Last().Date.ToShortDateString();
                data.Add(new { Average = avg, Date = date });
                average[j] = (int)avg;
                dateArray[j] = date.ToString();
                j++;
                i += averageOnDays;
            }

            return JsonConvert.SerializeObject(data);
        }


        public IndustryModel GetCompanies(int industryId)
        {
            using (var db = new DataStore<IndustryModel>("industry"))
            {
                var industry = db.GetCollection().Find(x => x.Id == industryId);
                if (industry == null)
                {
                    var json = _yqlService.FetchCompanies(industryId);
                    var jobj = JObject.Parse(json);
                    jobj = JObject.Parse(jobj["query"]["results"]["industry"].ToString());
                    industry = Helper.DeserializeJson<IndustryModel>(jobj);
                    SaveIndustry(industry);
                }

                return industry;
            }
        }

        public QuoteModel GetQuote(string symbol)
        {
            using (var db = new DataStore<QuoteModel>("quotes"))
            {
                var quote = db.GetCollection().Where(x => x.Symbol == symbol).FirstOrDefault();
                if (quote == null)
                {
                    var json = _yqlService.FetchQuote(symbol);
                    var jobj = JObject.Parse(json);
                    quote = Helper.DeserializeJson<QuoteModel>(JObject.Parse(jobj["query"]["results"]["quote"].ToString()));
                    SaveQuote(quote);

                }
                return quote;
            }
        }

        #region Save Methods
        public void SaveIndustry(IndustryModel industry)
        {
            using (var ds = new DataStore<IndustryModel>(IndustryCollection))
            {
                ds.SaveOne(industry);
            }
        }

        public void SaveQuote(QuoteModel quote)
        {
            using (var db = new DataStore<QuoteModel>("quotes"))
            {
                db.SaveOne(quote);
            }
        }

        public void SaveHistory(List<HistoryModel> histor)
        {

        }
        #endregion
    }
}
