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
using NCS.DSS.Customer.ReferenceData;
using System.Dynamic;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger
{
    public static class PostCustomerHttpTrigger
    {
        [FunctionName("POST")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Added", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Post request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [Response(HttpStatusCode = (int)422, Description = "Customer resource validation error(s)", ShowSchema = false)]
        [ResponseType(typeof(Models.Customer))]
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/")]HttpRequestMessage req, TraceWriter log)
        {
            var customerData = await req.Content.ReadAsAsync<Models.Customer>();
            var service = new PostCustomerHttpTriggerService();
            var customerId = service.CreateNewCustomer().CustomerID;
            customerData.CustomerID = customerId; 
            var cusJson = JsonConvert.SerializeObject(customerData, Formatting.Indented);

            return customerId == null
                ? new HttpResponseMessage(HttpStatusCode.BadRequest)
                : new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent("New customer ID created : " + customerId + Environment.NewLine + Environment.NewLine + cusJson)
                };
        }
    }
}
