using System.IO;
using System.Net;

namespace FinApp.MiddleWare
{
    public class Sector : FinBase
    {
        public Sector()
        {
            LoadSectors();
        }

        private void LoadSectors()
        {
            //Initial load of the sectors csv file
        }

        private void GetSectorsCsv(int id = 0)
        {
            WebRequest request = WebRequest.Create(BizCsvPath + Csv + CONAMEUCSV);
            var response = request.GetResponse();
            var csv = new StreamReader(response.GetResponseStream()).ReadToEnd();

        }

    }
}
