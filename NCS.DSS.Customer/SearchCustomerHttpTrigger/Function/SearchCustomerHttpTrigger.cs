using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;
using NCS.DSS.Customer.Annotations;
using NCS.DSS.Customer.AppInsights;
using Microsoft.Extensions.Logging;
using System;
using System.Web.Http;
using System.Linq;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger
{
    public static class SearchCustomerHttpTrigger
    {
        [FunctionName("SEARCH")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Retrieved", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Get request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [ResponseType(typeof(Models.Customer))]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get",
            Route = "CustomerSearch")]HttpRequestMessage req, TraceWriter log)
        {
            var service = new SearchCustomerHttpTriggerService();
            var values = service.SearchCustomer(req);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(values, Formatting.Indented),
                    System.Text.Encoding.UTF8, "application/json")
            };
        }

    }

}
