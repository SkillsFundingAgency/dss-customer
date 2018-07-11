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
using System;
using System.Web.Http;
using System.Linq;
using NCS.DSS.Customer.Ioc;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.GetCustomerHttpTrigger.Service;
using NCS.DSS.Customer.SearchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Helpers;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger
{
    public static class SearchCustomerHttpTrigger
    {
        [FunctionName("SEARCH")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Retrieved", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Get request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [ResponseType(typeof(Models.Customer))]
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get",
            Route = "CustomerSearch/{qQuery}")]HttpRequestMessage req, ILogger logger, string qQuery,
            [Inject]IResourceHelper resourceHelper,
            [Inject]ISearchCustomerHttpTriggerService SearchCustomerService)
        {
            logger.LogInformation("C# HTTP trigger function GetCustomerById processed a request.");

            var customer = await SearchCustomerService.SearchCustomerAsync(qQuery);

            return customer == null ?
                HttpResponseMessageHelper.NoContent(Guid.NewGuid()) :
                HttpResponseMessageHelper.Ok(customer);
        }

    }

}
