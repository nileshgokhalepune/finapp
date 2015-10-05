using FinApp.Helpers;
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
        private const string CompanySelectQuery = "Select * from yahoo.finance.industry where id = {0}";
        private const string QuoteSelectQuery = "Select * From yahoo.finance.quote where symbol = \"{0}\"";
        private const string HistorySelectQuery = "Select * From yahoo.finance.historicaldata where symbol=\"{0}\" and startDate=\"{1}\" and endDate = \"{2}\"";
        private DataStore _dataStore;
        private const string IndustryCollection = "industry";
        public StockManager()
        {
        }

        public IndustryModel GetCompanies(int industryId)
        {
            WebRequest request = WebRequest.Create(YqlPath + string.Format(CompanySelectQuery, industryId) + "&" + ENVPARAM + "&" + FORMATPARAM);
            var json = Helper.GetResponseText(request.GetResponse());
            var jobj = JObject.Parse(json);
            jobj = JObject.Parse(jobj["query"]["results"]["industry"].ToString());
            var list = Helper.DeserializeJson<IndustryModel>(jobj);
            return list;
        }

        public QuoteModel GetQuote(string symbol)
        {
            WebRequest request = WebRequest.Create(YqlPath + string.Format(QuoteSelectQuery, symbol) + "&" + ENVPARAM + "&" + FORMATPARAM);
            var json = Helper.GetResponseText(request.GetResponse());
            var jobj = JObject.Parse(json);
            return Helper.DeserializeJson<QuoteModel>(JObject.Parse(jobj["query"]["results"]["quote"].ToString()));
        }

        public List<HistoryModel> GetHistory(string symbol, DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.Now.AddMonths(-11);
                endDate = DateTime.Now;
            }
            var startDateParam = String.Format("{0:yyyy-MM-dd}", startDate.Value);// DateTime.ParseExact(startDate.Value.ToShortDateString(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentUICulture);
            var endDateParam = String.Format("{0:yyyy-MM-dd}", endDate.Value);// DateTime.ParseExact(endDate.Value.ToShortDateString(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentUICulture);
            WebRequest request = WebRequest.Create(YqlPath + string.Format(HistorySelectQuery, symbol, startDateParam, endDateParam) + "&" + ENVPARAM + "&" + FORMATPARAM);
            var json = Helper.GetResponseText(request.GetResponse());
            var jobj = JObject.Parse(json);
            return Helper.DeserializeJson<List<HistoryModel>>(JArray.Parse(jobj["query"]["results"]["quote"].ToString()));
        }

        public void SaveCompanies(IndustryModel industry)
        {
            using (_dataStore = new DataStore(IndustryCollection))
            {

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
            var orderdedData = model.OrderBy(h => h.Date);
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
                var date = orderdedData.Skip(i).Take(averageOnDays).Last().Date;
                data.Add(new { Average = avg, Date = date });
                average[j] = (int)avg;
                dateArray[j] = date.ToString();
                j++;
                i += averageOnDays;
            }

            return JsonConvert.SerializeObject(data);
        }

    }
}
