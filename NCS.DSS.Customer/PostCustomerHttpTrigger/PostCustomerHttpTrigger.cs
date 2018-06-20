using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.Customer;
using System.Web.Http.Description;
using System;
using NCS.DSS.Customer.Annotations;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger
{
    public static class PostCustomerHttpTrigger
    {
        [FunctionName("POST")]
        [CustomerResponse(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Customer Created", ShowSchema = true)]
        [CustomerResponse(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Unable to create Customer", ShowSchema = false)]
        [CustomerResponse(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Forbidden", ShowSchema = false)]
        [ResponseType(typeof(Models.Customer))]
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function Add Customer processed a request.");

            // Get request body
            var customer = await req.Content.ReadAsAsync<Models.Customer>();

            var service = new PostCustomerHttpTriggerService();
            var customerId = service.Create(customer);

            return customerId == null
                ? new HttpResponseMessage(HttpStatusCode.BadRequest)
                : new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent("Created Customer record with Id of : " + customerId)
                };
        }
    }
}
