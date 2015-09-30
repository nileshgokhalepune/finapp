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
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }

        [HttpGet]
        [Route("sectors", Name = "sectors")]
        public HttpResponseMessage GetSectors()
        {
            try
            {
                SectorManager manager = new SectorManager();

                return null;
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
        public HttpResponseMessage GetIndustries(string sectorID = "")
        {
            try
            {
                return null;
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


