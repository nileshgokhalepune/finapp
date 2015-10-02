using FinApp.Helpers;
using FinApp.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace FinApp.MiddleWare
{
    public class SectorManager : FinBase
    {
        private string SectorHtml { get; set; }

        public List<SectorModel> SectorList { get; set; }

        public SectorManager(bool loadDefault = false)
        {
            if (loadDefault)
                LoadSectors();
        }

        private void LoadSectors()
        {
                var sectors = GetSectorsCsv();
                SectorList = sectors;
        }

        public List<SectorModel> GetSector(int sectorId)
        {
            var list = GetSectorsCsv(sectorId);
            return list;
        }

        private List<SectorModel> GetSectorsCsv(int id = 0)
        {
            var sectorId = id == 0 ? CONAMEUSCS_PREFIX : id.ToString();
            WebRequest request = WebRequest.Create(BizCsvPath + sectorId + CONAMEUCSV);
            var csv = Helper.GetResponseText(request.GetResponse());
            var rows = csv.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<SectorModel> sectorsList = new List<SectorModel>();
            for (int i = 1; i < rows.Length; i++)
            {
                var columns = rows[i].Split(',');
                if (columns[0] == "\0") continue;
                var sectorid = GetSectorId(columns[0].Replace("\"", ""), id);
                sectorsList.Add(new SectorModel() { SectorName = columns[0].Replace("\"", ""), SectorId = sectorid });

            }

            return sectorsList;
        }

        private void FetchSectorHtml(string sectorId = "")
        {
            sectorId = string.IsNullOrEmpty(sectorId) ? CONAMEUSCS_PREFIX : sectorId;
            WebRequest request = WebRequest.Create(BizPath + sectorId + CONAMEUHTML);
            SectorHtml = Helper.GetResponseText(request.GetResponse());

        }

        private int GetSectorId(string sectorName, int parentSectorId = 0)
        {
            if (string.IsNullOrEmpty(SectorHtml)) FetchSectorHtml(parentSectorId == 0 ? "" : parentSectorId.ToString());
            var searchFor = "-1>" + sectorName;
            var fIndex = SectorHtml.IndexOf(searchFor);
            if (fIndex == -1)
            {
                fIndex = SectorHtml.IndexOf(searchFor.Replace(" ", System.Environment.NewLine));
                if (fIndex == -1) fIndex = SectorHtml.IndexOf(searchFor.Replace(" ", "\n"));
                if (fIndex == -1) return 0;
            }
            var sectortext = SectorHtml.Substring(0, fIndex);
            var startIndex = sectortext.LastIndexOf(CONAMEUHTML) - 1;
            var countNum = 1;
            for (int j = startIndex; ; j--)
            {
                int result;
                if (Int32.TryParse(sectortext[j].ToString(), out result))
                {
                    if (j < startIndex)
                    {
                        startIndex -= 1;
                        countNum++;
                    }
                }
                else
                {
                    break;
                }
            }
            var conname = sectortext.Substring(startIndex, CONAMEUHTML.Length + countNum);
            return Convert.ToInt32(conname.Substring(0, conname.IndexOf(CONAMEUHTML)));
        }


    }
}
