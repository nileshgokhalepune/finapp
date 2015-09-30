using FinApp.Helpers;
using FinApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace FinApp.MiddleWare
{
    public class SectorManager : FinBase
    {
        private string SectorHtml { get; set; }

        public List<SectorModel> SectorList { get; set; }

        public SectorManager()
        {
            LoadSectors();
        }

        private void LoadSectors()
        {
            //Initial load of the sectors csv file
            var sectors = GetSectorsCsv();
            foreach (var sector in sectors)
            {
                var id = GetSectorIds(sector.SectorName);
                sector.SectorId = id;
            }

            SectorList = sectors;
        }

        private List<SectorModel> GetSectorsCsv(int id = 0)
        {
            WebRequest request = WebRequest.Create(BizCsvPath + CONAMEUSCS_PREFIX + CONAMEUCSV);
            var csv = Helper.GetResponseText(request.GetResponse());
            var rows = csv.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<SectorModel> sectorsList = new List<SectorModel>();
            for (int i = 1; i < rows.Length; i++)
            {
                var columns = rows[i].Split(',');
                sectorsList.Add(new SectorModel() { SectorName = columns[0].Replace("\"", "") });
            }

            return sectorsList;
        }

        private void FetchSectorHtml()
        {
            WebRequest request = WebRequest.Create(BizPath + CONAMEUSCS_PREFIX + CONAMEUHTML);
            SectorHtml = Helper.GetResponseText(request.GetResponse());

        }

        private int GetSectorIds(string sectorName)
        {
            if (string.IsNullOrEmpty(SectorHtml)) FetchSectorHtml();
            var fIndex = SectorHtml.IndexOf(sectorName);
            if (fIndex == -1)
            {
                fIndex = SectorHtml.IndexOf(sectorName.Replace(" ", System.Environment.NewLine));
                if (fIndex == -1) fIndex = SectorHtml.IndexOf(sectorName.Replace(" ", "\n"));
                if (fIndex == -1) return 0;
            }
            var sectortext = SectorHtml.Substring(0, fIndex);
            var startIndex = sectortext.LastIndexOf(CONAMEUHTML) - 1;
            var conname = sectortext.Substring(startIndex, CONAMEUHTML.Length + 1);
            return Convert.ToInt32(conname.Substring(0, 1));
        }


    }
}
