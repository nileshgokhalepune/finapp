using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp.MiddleWare
{
    public class FinBase
    {
        protected readonly string YqlPath;
        protected readonly string BizPath;
        protected readonly string BizCsvPath;
        protected const string Csv = "csv/";
        protected const string CONAMEUHTML = "conameu.html";
        protected const string CONAMEUCSV = "conameu.csv";
        protected const string CONAMEUSCS_PREFIX = "s_";


        protected FinBase()
        {
            YqlPath = ConfigurationManager.AppSettings["yqlUrl"];
            BizPath = ConfigurationManager.AppSettings["yahooBizUrl"];
            BizCsvPath = BizPath + Csv;
        }

    }
}
