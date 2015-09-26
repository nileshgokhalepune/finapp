using System;
using System.IO;
using System.Net;
using System.Text;
using FinApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace FinApp.MiddleWare
{
    public enum YqlTables
    {
        [StringValue("yahoo.finance.sectors")]
        YahooFinanceSectors,
        [StringValue("yahoo.finance.industry")]
        YahooFinanceIndustries,
        [StringValue("yahoo.finance.quotes")]
        YahooFinanceQuotes,
        [StringValue("yahoo.finance.historicaldata")]
        YahooFinanceHistoricalData
    }

    public class YqlManager
    {
        private const string BaseUrl = "http://query.yahooapis.com/v1/public/yql?q=";
        private const string FormatParam = "&format=json";
        private const string Select = "Select * From {0} Where";
        private const string EnvParam = "&env=store://datatables.org/alltableswithkeys";
        private WebRequest _yqlRequest = null;
        public YqlManager()
        {

        }

        /// <summary>
        /// Gets the industries.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public IndustryModel GetIndustries(string id = "")
        {
            var dynamicData = GetObjectsResults(StringValueAttribute.GetString(YqlTables.YahooFinanceIndustries).ToString(), new[] { "id=" + id });
            return DeserializeJson<IndustryModel>(JObject.Parse(dynamicData["industry"].ToString()));

        }

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public QuoteModel GetHistory(string symbol)
        {
            var dynamicData = GetObjectsResults(StringValueAttribute.GetString(YqlTables.YahooFinanceHistoricalData).ToString(),
                new[] { "symbol=" + symbol });
            return DeserializeJson<QuoteModel>(JObject.Parse(dynamicData["quote"].ToString()));
        }

        private JObject GetObjectsResults(string tableName, string[] whereClause)
        {
            string jsonData = string.Empty;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append(string.Format(Select, tableName));
            if (whereClause != null && whereClause.Length > 0)
            {
                foreach (var whereCon in whereClause)
                {
                    var where = " " + whereCon + " AND ";
                    selectQuery.Append(where);
                }

                selectQuery.Remove(selectQuery.ToString().LastIndexOf("AND", StringComparison.Ordinal), 3);
            }
            else
            {
                selectQuery.Append(" 1=1 ");
            }

            _yqlRequest = WebRequest.Create(BaseUrl + selectQuery.ToString().Trim() + FormatParam + EnvParam);
            var response = _yqlRequest.GetResponse().GetResponseStream();
            if (response != null)
            {
                jsonData = new StreamReader(response).ReadToEnd();

            }


            return !string.IsNullOrEmpty(jsonData) ? GetResults(jsonData) : null;
        }

        private JObject GetResults(string jsonData)
        {
            var serializer = JsonSerializer.Create();
            JObject data = serializer.Deserialize(new JsonTextReader(new StringReader(jsonData))) as JObject;
            return JObject.Parse(data["query"]["results"].ToString()); //data.query.results;
        }

        private T DeserializeJson<T>(JObject jsonData)
        {
            var serializer = JsonSerializer.Create();
            return serializer.Deserialize<T>(new JsonTextReader(new StringReader(jsonData.ToString())));
        }
    }
}
