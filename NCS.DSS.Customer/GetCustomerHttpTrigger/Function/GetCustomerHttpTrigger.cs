using DFC.Functions.DI.Standard.Attributes;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Annotations;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.GetCustomerHttpTrigger.Service;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.GetCustomerHttpTrigger.Function
{
    public static class GetCustomerHttpTrigger
    {
        [FunctionName("GET")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Retrieved", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Get request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        [Disable]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers")]HttpRequestMessage req, ILogger log,
                [Inject]IResourceHelper resourceHelper,
                [Inject]IGetCustomerHttpTriggerService getAllCustomerService,
                [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
                [Inject]IJsonHelper jsonHelper)
        {
            var customer = await getAllCustomerService.GetAllCustomerAsync();

            return customer == null ?
                httpResponseMessageHelper.NoContent(Guid.NewGuid()) :
                httpResponseMessageHelper.Ok(jsonHelper.SerializeObjectsAndRenameIdProperty(customer, "id", "CustomerId"));
        }
    }
}
