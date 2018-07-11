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
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Ioc;
using Newtonsoft.Json;

namespace NCS.DSS.Customer.GetCustomerByIdHttpTrigger
{
    public static class GetCustomerByIdHttpTrigger
    {
        [FunctionName("GETByID")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Retrieved", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Get request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [ResponseType(typeof(Models.Customer))]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", 
            Route = "Customers/{customerId}")]HttpRequestMessage req, TraceWriter log, string customerId,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IGetCustomerByIdHttpTriggerService getCustomerByIdService)
        {
            log.Info("C# HTTP trigger function GetCustomerById processed a request.");

            if (!Guid.TryParse(customerId, out var customerGuid))
                return HttpResponseMessageHelper.BadRequest(customerGuid);

            var doesCustomerExist = resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
                return HttpResponseMessageHelper.NoContent(customerGuid);

            var customer = getCustomerByIdService.GetCustomerAsync(customerGuid);

            return customer == null ?
                HttpResponseMessageHelper.NoContent(customerGuid) :
                HttpResponseMessageHelper.Ok(customer);

        }
    }
}
