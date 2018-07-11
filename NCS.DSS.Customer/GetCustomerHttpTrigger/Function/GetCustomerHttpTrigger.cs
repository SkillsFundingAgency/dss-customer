using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;
using NCS.DSS.Customer.Annotations;
using NCS.DSS.Customer.AppInsights;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.GetCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NCS.DSS.Customer.Ioc;
using NCS.DSS.Customer.Helpers;
using System;

namespace NCS.DSS.Customer.GetCustomerHttpTrigger
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
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get",
            Route = "Customers")]HttpRequestMessage req, TraceWriter log,
                [Inject]IResourceHelper resourceHelper,
                [Inject]IGetCustomerHttpTriggerService getAllCustomerService)
        {
            log.Info("C# HTTP trigger function GetCustomerById processed a request.");

            var customer = getAllCustomerService.GetAllCustomerAsync();

            return customer == null ?
                HttpResponseMessageHelper.NoContent(Guid.NewGuid()) :
                HttpResponseMessageHelper.Ok(customer);

        }
    }
}
