using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace NCS.CDS.Customer.GetCustomerHttpTrigger
{
    public static class GetCustomerHttpTrigger
    {
        [FunctionName("GetCustomer")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customer")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function GetCustomer processed a request.");

            var service = new GetCustomerHttpTriggerService();
            var values = await service.GetCustomer();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(values),
                    System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
