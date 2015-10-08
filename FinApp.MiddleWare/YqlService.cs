using FinApp.Helpers;
using System;
using System.Net;

namespace FinApp.MiddleWare
{
    public class YqlService : FinBase
    {
        private const string CompanySelectQuery = "Select * from yahoo.finance.industry where id = {0}";
        private const string QuoteSelectQuery = "Select * From yahoo.finance.quote where symbol = \"{0}\"";
        private const string HistorySelectQuery = "Select * From yahoo.finance.historicaldata where symbol=\"{0}\" and startDate=\"{1}\" and endDate = \"{2}\"";

        public string FetchCompanies(int industryId)
        {
            WebRequest request = WebRequest.Create(YqlPath + string.Format(CompanySelectQuery, industryId) + "&" + ENVPARAM + "&" + FORMATPARAM);
            var json = Helper.GetResponseText(request.GetResponse());
            return json;
        }

        public string FetchQuote(string symbol)
        {
            WebRequest request = WebRequest.Create(YqlPath + string.Format(QuoteSelectQuery, symbol) + "&" + ENVPARAM + "&" + FORMATPARAM);
            var json = Helper.GetResponseText(request.GetResponse());
            return json;
        }

        public string FetchHistory(string symbol, DateTime startDate, DateTime endDate)
        {
            var startDateParam = String.Format("{0:yyyy-MM-dd}", startDate);// DateTime.ParseExact(startDate.Value.ToShortDateString(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentUICulture);
            var endDateParam = String.Format("{0:yyyy-MM-dd}", endDate);// DateTime.ParseExact(endDate.Value.ToShortDateString(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentUICulture);
            WebRequest request = WebRequest.Create(YqlPath + string.Format(HistorySelectQuery, symbol, startDateParam, endDateParam) + "&" + ENVPARAM + "&" + FORMATPARAM);
            var json = Helper.GetResponseText(request.GetResponse());
            return json;
        }
    }
}
