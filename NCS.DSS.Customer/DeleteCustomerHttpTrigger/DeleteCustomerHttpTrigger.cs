using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace NCS.DSS.Customer.DeleteCustomerHttpTrigger
{
    public static class DeleteCustomerHttpTrigger
    {
        [FunctionName("DeleteCustomers")]
        [ResponseType(typeof(Models.Customer))]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "customers/{customerId}")]HttpRequestMessage req, TraceWriter log, string customerId)
        {
            log.Info("C# HTTP trigger function GetCustomer processed a request.");

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Deleted record with Id of : " + customerId)
            };
        }
    }
}
