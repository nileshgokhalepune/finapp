using System.IO;
using System.Net;

namespace FinApp.MiddleWare
{
    public class SectorManager : FinBase
    {
        public SectorManager()
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
