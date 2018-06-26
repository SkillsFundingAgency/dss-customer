using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;
using NCS.DSS.Customer.Annotations;

namespace NCS.DSS.Customer.GetCustomerHttpTrigger
{
    public static class GetCustomerHttpTrigger
    {
        [FunctionName("GET")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Retrieved", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Get request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [ResponseType(typeof(Models.Customer))]
        [Disable]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function GetCustomer processed a request.");

            var service = new GetCustomerHttpTriggerService();
            var values = await service.GetCustomer();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(values, Formatting.Indented),
                    System.Text.Encoding.UTF8, "application/json")
            };
        }
    }

}
