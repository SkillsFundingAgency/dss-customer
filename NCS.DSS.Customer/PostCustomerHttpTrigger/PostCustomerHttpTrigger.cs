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

namespace NCS.DSS.Customer.PostCustomerHttpTrigger
{
    public static class PostCustomerHttpTrigger
    {
        [FunctionName("AddCustomer")]
        [ResponseType(typeof(Models.Customer))]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function Add Customer processed a request.");

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Added customer record with Id of : " + Guid.NewGuid())
            };
        }
    }
}
