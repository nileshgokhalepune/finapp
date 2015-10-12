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
                if (startDate == null || endDate == null)
                {
                    startDate = DateTime.Now.AddYears(-3);
                    endDate = DateTime.Now;
                }

                var history = db.GetCollection().Where(x => x.Symbol == symbol && x.Date >= startDate.Value && x.Date <= endDate.Value);
                if (!history.Any())
                {
                    var json = _yqlService.FetchHistory(symbol, startDate.Value, endDate.Value);
                    var jobj = JObject.Parse(json);
                    if (Convert.ToInt32(((JValue)(jobj["query"]["count"])).Value) > 0)
                    {
                        history = Helper.DeserializeJson<List<HistoryModel>>(JArray.Parse(jobj["query"]["results"]["quote"].ToString()));
                        history.ToList().ForEach(x => x.Symbol = symbol);
                        SaveHistory(history.ToList());
                    }
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
            var sma = (from m in model
                       let avg = model.Where(x => x.Date <= startDate && x.Date >= m.Date.AddDays(-averageOnDays)).Average(x => x.Close)
                       select new
                       {
                           Average = avg,
                           Date = m.Date
                       });

            var groupedByMonth = (from s in sma.ToList()
                                  group s by new { s.Date.Month, s.Date.Year } into grp
                                  select new
                                  {
                                      Date = grp.Key.Month,
                                      Average = grp.Average(x => x.Average)
                                  });
            return JsonConvert.SerializeObject(groupedByMonth.ToList());
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
                    industry.Company.ForEach(x => x.SectorId = industry.Id);
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

        public void SaveHistory(List<HistoryModel> history)
        {
            using (var db = new DataStore<HistoryModel>("history"))
            {
                db.SaveMany(history);
            }
        }
        #endregion
    }
}
