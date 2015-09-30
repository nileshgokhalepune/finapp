using FinApp.MiddleWare;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FinApp.Controllers
{
    [RoutePrefix("api/data")]
    public class YqlApiController : ApiController
    {
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
                return Request.CreateResponse(HttpStatusCode.OK, manager.SectorList);
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
                SectorManager manager = new SectorManager();
                var industries = manager.GetSector(sectorID);
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
                return null;
                //YqlManager manager = new YqlManager();
                //var quotes = manager.GetHistory(symbol);
                //return Request.CreateResponse(HttpStatusCode.OK, quotes);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}


