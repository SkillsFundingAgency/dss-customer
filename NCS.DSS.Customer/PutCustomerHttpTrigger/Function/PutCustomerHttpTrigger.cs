using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Annotations;
using System.Net;
using System.Net.Http;

namespace NCS.DSS.Customer.PutCustomerHttpTrigger
{
    public static class PutCustomerHttpTrigger
    {
        [FunctionName("PUT")]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Customer Replaced", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Unable to Replace Customer", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "Unauthorised", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        [Disable]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Customers/{customerId}")]HttpRequestMessage req, ILogger log, string customerId,
            [Inject]ILoggerHelper loggerHelper)
        {
            loggerHelper.LogMethodEnter(log);            

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Replaced customer record with Id of : " + customerId)
            };
        }
    }
}
