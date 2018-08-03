using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Annotations;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.GetCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Ioc;

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
        [ResponseType(typeof(Models.Customer))]
        [Disable]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers")]HttpRequestMessage req, ILogger log,
                [Inject]IResourceHelper resourceHelper,
                [Inject]IGetCustomerHttpTriggerService getAllCustomerService)
        {
            var customer = await getAllCustomerService.GetAllCustomerAsync();

            return customer == null ?
                HttpResponseMessageHelper.NoContent(Guid.NewGuid()) :
                HttpResponseMessageHelper.Ok(JsonHelper.SerializeObject(customer));

        }
    }
}
