using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;
using NCS.DSS.Customer.Annotations;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger
{
    public static class PatchCustomerHttpTrigger
    {
        [FunctionName("PATCH")]
        [CustomerResponse(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Customer Updated", ShowSchema = true)]
        [CustomerResponse(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Unable to update Customer", ShowSchema = false)]
        [CustomerResponse(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Forbidden", ShowSchema = false)]
        [ResponseType(typeof(Models.Customer))]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Customers/{customerId}")]HttpRequestMessage req, TraceWriter log, string customerId)
        {
            log.Info("C# HTTP trigger function Update Customer processed a request.");

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Updated customer record with Id of : " + customerId)
            };
        }
    }
}
