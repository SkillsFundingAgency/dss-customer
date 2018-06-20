using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using NCS.DSS.Customer.Annotations;
using Newtonsoft.Json;

namespace NCS.DSS.Customer.GetCustomerByIdHttpTrigger
{
    public static class GetCustomerByIdHttpTrigger
    {
        [FunctionName("GETByID")]
        [CustomerResponse(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Customer Retrieved", ShowSchema = true)]
        [CustomerResponse(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Unable to Retrive Customer", ShowSchema = false)]
        [CustomerResponse(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Forbidden", ShowSchema = false)]
        [ResponseType(typeof(Models.Customer))]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}")]HttpRequestMessage req, TraceWriter log, string customerId)
        {
            log.Info("C# HTTP trigger function GetCustomerById processed a request.");

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(customerId),
                        System.Text.Encoding.UTF8, "application/json")
                };
            }
            var service = new GetCustomerByIdHttpTriggerService();
            var values = await service.GetCustomer(customerGuid);


            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(values),
                    System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
