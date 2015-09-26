using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FinApp.MiddleWare;

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
        public HttpResponseMessage GetSectors(string scripName = "")
        {
            try
            {
                YqlManager manager = new YqlManager();
                var list = manager.GetIndustries(112.ToString());
                return Request.CreateResponse(HttpStatusCode.OK, list);
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
                YqlManager manager = new YqlManager();
                
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}


