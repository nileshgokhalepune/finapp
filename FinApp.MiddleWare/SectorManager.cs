using FinApp.Helpers;
using FinApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FinApp.MiddleWare
{
    public class SectorManager : FinBase
    {
        private YqlService _yqlService;
        private string SectorHtml { get; set; }

        public List<SectorModel> SectorList { get; set; }

        public SectorManager(bool loadDefault = false)
        {
            _yqlService = new YqlService();
            if (loadDefault)
                LoadSectors();
        }

        private void LoadSectors()
        {
            List<SectorModel> sectors;
            sectors = GetCachedSectors();
            if (sectors == null || sectors.Count <= 0)
            {
                sectors = GetSubSectors();
                SaveSectors(sectors);

            }
            SectorList = sectors;
        }

        public List<SectorModel> GetSector(int sectorId)
        {
            List<SectorModel> lst = null;
            using (var db = new DataStore<SectorModel>("subsectors"))
            {
                lst = db.GetCollection().Where(x => x.ParentSectorId == sectorId).ToList();
            }
            if (lst != null && !lst.Any())
            {
                lst = GetSubSectors(sectorId);
                SaveSubSectors(lst);
            }
            return lst;
        }

        private List<SectorModel> GetSubSectors(int id = 0)
        {
            var csvData = _yqlService.FetchSubSectorsData(id);
            var rows = csvData.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<SectorModel> sectorsList = new List<SectorModel>();
            for (int i = 1; i < rows.Length; i++)
            {
                var columns = rows[i].Split(',');
                if (columns[0] == "\0") continue;
                var sectorid = GetSectorId(columns[0].Replace("\"", ""), id);
                sectorsList.Add(new SectorModel() { ParentSectorId = id, SectorName = columns[0].Replace("\"", ""), SectorId = sectorid });

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

        public List<SectorModel> GetCachedSectors()
        {
            using (var db = new DataStore<SectorModel>("sectors"))
            {
                return db.GetCollection();
            }
        }

        public void SaveSectors(List<SectorModel> sectors)
        {
            using (var db = new DataStore<SectorModel>("sectors"))
            {
                db.SaveMany(sectors);
            }
        }

        public void SaveSubSectors(List<SectorModel> list)
        {
            using (var db = new DataStore<SectorModel>("subsectors"))
            {
                db.SaveMany(list);
            }
        }
    }
}
