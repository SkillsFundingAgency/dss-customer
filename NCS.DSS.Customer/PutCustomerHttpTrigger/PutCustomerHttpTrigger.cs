using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace NCS.DSS.Customer.PutCustomerHttpTrigger
{
    public static class PutCustomerHttpTrigger
    {
        [FunctionName("ReplaceCustomer")]
        [ResponseType(typeof(Models.Customer))]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "customers/{customerId}")]HttpRequestMessage req, TraceWriter log, string customerId)
        {
            log.Info("C# HTTP trigger function Replace Customer processed a request.");

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Replaced customer record with Id of : " + customerId)
            };
        }
    }
}
