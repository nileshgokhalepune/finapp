using FinApp.MiddleWare;
using FinApp.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Caching;
using System.Web.Http;
using System.Linq;

namespace FinApp.Controllers
{
    [RoutePrefix("api/data")]
    public class YqlApiController : ApiController
    {
        private static object lockObject = new Object();
        [HttpGet]
        [Route("checkSiteStatus", Name = "checkSiteStatus")]
        public HttpResponseMessage CheckSiteStatus()
        {
            try
            {
                WebRequest yqlRequest = WebRequest.Create(new Uri("http://query.yahooapis.com/v1/public/yql"));
                var yqlResponse = yqlRequest.GetResponse();
                return Request.CreateResponse(HttpStatusCode.OK);
            }

#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }

        [HttpGet]
        [Route("sectors", Name = "sectors")]
        public HttpResponseMessage GetSectors(int sectorId = 0)
        {
            try
            {
                SectorManager manager = new SectorManager(true);
                lock (lockObject)
                {
                    if (System.Web.HttpContext.Current.Cache["Sectors"] == null)
                    {
                        System.Web.HttpContext.Current.Cache.Add("Sectors", manager.SectorList, null, DateTime.Now.AddMinutes(20), TimeSpan.Zero, CacheItemPriority.Default, null);
                    }
                }
                var list = System.Web.HttpContext.Current.Cache["Sectors"] as List<SectorModel>;
                return Request.CreateResponse(HttpStatusCode.OK, list);
                //YqlManager manager = new YqlManager();
                //return Request.CreateResponse(HttpStatusCode.OK, manager.CurrentSectors);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("industry", Name = "industry")]
        public HttpResponseMessage GetIndustries(int sectorID = 0)
        {
            try
            {
                List<SectorModel> industries;
                SectorManager manager = new SectorManager();
                if (System.Web.HttpContext.Current.Cache[sectorID.ToString()] != null)
                {
                    industries = System.Web.HttpContext.Current.Cache[sectorID.ToString()] as List<SectorModel>;
                }
                else
                {
                    industries = manager.GetSector(sectorID);
                }
                System.Web.HttpContext.Current.Cache.Add(sectorID.ToString(), industries, null, DateTime.Now.AddMinutes(20), TimeSpan.Zero, CacheItemPriority.Default, null);
                return Request.CreateResponse(HttpStatusCode.OK, industries);
                //SectorManager manager = new SectorManager();
                ////var list = manager.GetSubSector(sectorID);
                //if (list.Count > 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK, new { HasSectors = true, Sectors = list });
                //}
                //else
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK, list);
                //}

                //YqlManagerB manager = new YqlManager();
                //var list = manager.GetIndustries(sectorID);
                //return Request.CreateResponse(HttpStatusCode.OK, list);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("stocks", Name = "stocks")]
        public HttpResponseMessage GetStocks(int id)
        {
            StockManager manager = new StockManager();
            var list = manager.GetCompanies(id);
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpGet]
        [Route("history", Name = "history")]
        public HttpResponseMessage GetHistoricalData(string symbol)
        {
            try
            {
                List<HistoryModel> historyData;
                StockManager manager = new StockManager();
                if (System.Web.HttpContext.Current.Cache[symbol + "history"] != null)
                {
                    historyData = System.Web.HttpContext.Current.Cache[symbol + "history"] as List<HistoryModel>;
                }
                else
                {
                    historyData = manager.GetHistory(symbol, null, null);
                    System.Web.HttpContext.Current.Cache.Add(symbol + "history", historyData, null, DateTime.Now.AddMinutes(20), TimeSpan.Zero, CacheItemPriority.Default, null);
                }
                return Request.CreateResponse(HttpStatusCode.OK, historyData);
                //YqlManager manager = new YqlManager();
                //var quotes = manager.GetHistory(symbol);
                //return Request.CreateResponse(HttpStatusCode.OK, quotes);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("quote", Name = "quote")]
        public HttpResponseMessage GetQuote(string symbol)
        {
            try
            {
                StockManager manager = new StockManager();
                var quote = manager.GetQuote(symbol);
                return Request.CreateResponse(HttpStatusCode.OK, quote);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("trend", Name = "trend")]
        public HttpResponseMessage ShowTrend(string symbol)
        {
            var historyData = System.Web.HttpContext.Current.Cache[symbol + "history"] as List<HistoryModel>;
            try
            {
                //var result = historyData.GroupBy((t) => t.Date);
                var result = (from h in historyData
                              group h by new { Month = String.Format("{0:MMM}", new DateTime(h.Date.Year, h.Date.Month, 1)), MonNum = h.Date.Month, Year = h.Date.Year } into g
                              select new
                              {
                                  Date = new DateTime(g.Key.Year, g.Key.MonNum, 1),
                                  MonthWord = g.Key.Month,
                                  month = g.Key.MonNum,
                                  High = g.Sum(x => x.High),
                                  Low = g.Sum(x => x.Low),
                                  Open = g.Sum(x => x.Open),
                                  Close = g.Sum(x => x.Close)
                              }).OrderBy(x => x.Date).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.OK, ex);
            }

        }
        [HttpGet]
        [Route("monthtrend", Name = "monthtrend")]
        public HttpResponseMessage MonthTrend(string symbol, int month, int year)
        {
            var historyData = System.Web.HttpContext.Current.Cache[symbol + "history"] as List<HistoryModel>;
            try
            {
                var result = (from h in historyData
                              where h.Date.Year == year && h.Date.Month == month
                              select new
                              {
                                  Day = h.Date,
                                  High = h.High,
                                  Low = h.Low,
                                  Open = h.Open,
                                  Close = h.Close,
                              }).OrderBy(h => h.Day).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

        }
    }
}


