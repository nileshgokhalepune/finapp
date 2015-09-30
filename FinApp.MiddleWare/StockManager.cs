using FinApp.Helpers;
using FinApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace FinApp.MiddleWare
{
    public class StockManager : FinBase
    {
        private const string CompanySelectQuery = "Select * from yahoo.finance.industry where id = {0}";
        private const string QuoteSelectQuery = "Select * From yahoo.finance.quote where symbol = \"{0}\"";

        public IndustryModel GetCompanies(int industryId)
        {
            WebRequest request = WebRequest.Create(YqlPath + string.Format(CompanySelectQuery, industryId) + "&" + ENVPARAM + "&" + FORMATPARAM);
            var json = Helper.GetResponseText(request.GetResponse());
            var jobj = JObject.Parse(json);
            jobj = JObject.Parse(jobj["query"]["results"]["industry"].ToString());
            var list = Helper.DeserializeJson<IndustryModel>(jobj);
            return list;
        }

        public void GetQuote(string symbol)
        {
            WebRequest request = WebRequest.Create(YqlPath + string.Format(QuoteSelectQuery, symbol) + "&" + ENVPARAM + "&" + FORMATPARAM);
            var json = Helper.GetResponseText(request.GetResponse());
        }
    }
}
