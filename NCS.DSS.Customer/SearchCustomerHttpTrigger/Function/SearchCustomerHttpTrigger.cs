using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Annotations;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Ioc;
using NCS.DSS.Customer.SearchCustomerHttpTrigger.Service;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger.Function
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
        [Display(Name = "SEARCH", Description = "Ability to search for customers using query strings: \n Examples \n ?GivenName=Fred \n ?FamilyName=Bloggs \n ?DateofBirth=2018-01-01 \n ?UniqueLearnerNumber=0123456789 \n" +
                                                "You can also query a customer on multiple fields: \n Examples: \n ?GivenName=Fred&FamilyName=Bloggs \n ?UniqueLearnerNumber=0123456789&DateofBirth=2018-01-01")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get",
            Route = "CustomerSearch")]HttpRequestMessage req, ILogger log,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IHttpRequestMessageHelper httpRequestMessageHelper,
            [Inject]ISearchCustomerHttpTriggerService searchCustomerService)
        {
            var touchpointId = httpRequestMessageHelper.GetTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                log.LogInformation("Unable to locate 'APIM-TouchpointId' in request header");
                return HttpResponseMessageHelper.BadRequest();
            }

            log.LogInformation("C# HTTP trigger function GetCustomerById processed a request. By Touchpoint " + touchpointId);

            // Parse query parameter
            var givenName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "GivenName", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            var familyName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "FamilyName", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            var dateofBirth = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "DateofBirth", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            var uniqueLearnerNumber = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "UniqueLearnerNumber", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            var customer = await searchCustomerService.SearchCustomerAsync(givenName, familyName, dateofBirth, uniqueLearnerNumber);

            return customer == null ?
                HttpResponseMessageHelper.NoContent(Guid.NewGuid()) :
                HttpResponseMessageHelper.Ok(JsonHelper.SerializeObjects(customer));
        }

    }

}
