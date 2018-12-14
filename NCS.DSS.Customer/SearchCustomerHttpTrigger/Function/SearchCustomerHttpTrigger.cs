using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
        [Display(Name = "SEARCH", Description = "Ability to partially search for customers using query strings: </br> Examples </br> ?GivenName=Fred </br> ?FamilyName=Bloggs </br> ?DateofBirth=2018-01-01 </br> ?UniqueLearnerNumber=0123456789 </br>" +
                                                "You can also query a customer on multiple fields: </br> Examples: </br> ?GivenName=Fred&FamilyName=Bloggs </br> ?UniqueLearnerNumber=0123456789&DateofBirth=2018-01-01 </br>" +
                                                "When searching by Given Name or Family Name you need to supply a minimum of 2 characters")]
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

            SearchHelper.GetSearchServiceClient();
            var indexClient = SearchHelper.GetIndexClient();

            var query = string.Empty;
            var filter = string.Empty;

            // Parse query parameter
            var givenName = httpRequestMessageHelper.GetQueryNameValuePairs(req, "GivenName");
            var familyName = httpRequestMessageHelper.GetQueryNameValuePairs(req, "FamilyName");

            if (givenName != null && givenName.Length < 3)
            {
                log.LogWarning("Given Name must have a minimum of 3 characters");
                return HttpResponseMessageHelper.NoContent();
            }

            if (!string.IsNullOrEmpty(givenName))
            {
                query += string.Format("GivenName:({0}* OR {0}) ",givenName);
            }

            if (familyName != null && familyName.Length < 3)
            {
                log.LogWarning("Family Name must have a minimum of 3 characters");
                return HttpResponseMessageHelper.NoContent();
            }

            if (!string.IsNullOrEmpty(familyName))
            {
                query += string.Format("FamilyName:({0}* OR {0}) ", familyName);
            }

            var uniqueLearnerNumber = httpRequestMessageHelper.GetQueryNameValuePairs(req, "UniqueLearnerNumber");

            if (!string.IsNullOrEmpty(uniqueLearnerNumber))
            {
                query += string.Format("UniqueLearnerNumber:{0}", uniqueLearnerNumber);
            }

            var dob = httpRequestMessageHelper.GetQueryNameValuePairs(req, "DateofBirth");

            if (!string.IsNullOrEmpty(dob))
            {
                if (DateTime.TryParse(dob, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateOfBirth))
                    filter = string.Format("DateofBirth eq {0:yyyy-MM-dd}", dateOfBirth);
                else
                    return HttpResponseMessageHelper.NoContent();
            }

            if(string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(filter))
                return HttpResponseMessageHelper.NoContent();

            log.LogInformation("Attempting to search customers");

            var customer = await searchCustomerService.SearchCustomerAsync(indexClient, query, filter);

            log.LogInformation("Search completed");

            return customer == null ?
                HttpResponseMessageHelper.NoContent() :
                HttpResponseMessageHelper.Ok(JsonHelper.SerializeObjects(customer));
        }

    }
}