using FinApp.Helpers;
using FinApp.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace FinApp.MiddleWare
{
    public class StockManager : FinBase
    {
        private const string SelectQuery = "Select * from yahoo.finance.industry where id = {0}";

        public List<CompanyModel> GetCompanies(int industryId)
        {
            WebRequest request = WebRequest.Create(YqlPath + string.Format(SelectQuery, industryId) + "&" + ENVPARAM + "&" + FORMATPARAM);
            var csv = Helper.GetResponseText(request.GetResponse());
            var rows = csv.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < rows.Length; i++)
            {
                var columns = rows[i].Split(',');

            }
            return null;
        }
    }
}
