using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace NCS.DSS.Customer.ReferenceData
{
    public static class ReferenceDataMethods
    {
        [FunctionName("GetCustomerReferenceData")]
        [ResponseType(typeof(ReferenceData))]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "referencedata")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("GetCustomerReferenceData HTTP trigger function processed a request.");

            var service = new ReferenceDataService();
            var values = service.GetReferenceDataTypes();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(values),
                    System.Text.Encoding.UTF8, "application/json")
            };

        }
    }
}
