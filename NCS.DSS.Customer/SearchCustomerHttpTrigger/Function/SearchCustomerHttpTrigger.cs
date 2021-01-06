using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.SearchCustomerHttpTrigger.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger.Function
{
    public class SearchCustomerHttpTrigger
    {
        private readonly ILogger _log;
        private readonly IResourceHelper _resourceHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly ISearchCustomerHttpTriggerService _searchCustomerService;
        private readonly IHttpResponseMessageHelper _httpResponseMessageHelper;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILoggerHelper _loggerHelper;

        public SearchCustomerHttpTrigger(ILogger log,
             IResourceHelper resourceHelper,
             IHttpRequestHelper httpRequestHelper,
             ISearchCustomerHttpTriggerService searchCustomerService,
             IHttpResponseMessageHelper httpResponseMessageHelper,
             IJsonHelper jsonHelper,
             ILoggerHelper loggerHelper)
        {
            _log = log;
            _resourceHelper = resourceHelper;
            _httpRequestHelper = httpRequestHelper;
            _searchCustomerService = searchCustomerService;
            _httpResponseMessageHelper = httpResponseMessageHelper;
            _jsonHelper = jsonHelper;
            _loggerHelper = loggerHelper;
        }

        [FunctionName("SEARCH")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Retrieved", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Get request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        [Display(Name = "SEARCH", Description = "Ability to partially search for customers using query strings: </br> Examples </br> ?GivenName=Fred </br> ?FamilyName=Bloggs </br> ?DateofBirth=2018-01-01 </br> ?UniqueLearnerNumber=0123456789 </br>" +
                                                "You can also query a customer on multiple fields: </br> Examples: </br> ?GivenName=Fred&FamilyName=Bloggs </br> ?UniqueLearnerNumber=0123456789&DateofBirth=2018-01-01 </br>" +
                                                "When searching by Given Name or Family Name you need to supply a minimum of 2 characters")]
        public async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get",
            Route = "CustomerSearch")]HttpRequest req)
        {
            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                _log.LogInformation("Unable to locate 'DssCorrelationId; in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                _log.LogInformation("Unable to Parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                _loggerHelper.LogInformationMessage(_log, correlationGuid, "Unable to locate 'APIM-TouchpointId' in request header");
                return _httpResponseMessageHelper.BadRequest();
            }

            _loggerHelper.LogInformationMessage(_log, correlationGuid, "C# HTTP trigger function GetCustomerById processed a request. By Touchpoint " + touchpointId);

            SearchHelper.GetSearchServiceClient();
            var indexClient = SearchHelper.GetIndexClient();

            var query = string.Empty;
            var filter = string.Empty;

            // Parse query parameter
            var givenName = _httpRequestHelper.GetQueryString(req, "GivenName");
            var familyName = _httpRequestHelper.GetQueryString(req, "FamilyName");

            if (givenName != null && givenName.Length < 3)
            {
                _log.LogWarning("Given Name must have a minimum of 3 characters");
                return _httpResponseMessageHelper.NoContent();
            }

            if (!string.IsNullOrEmpty(givenName))
            {
                query += string.Format("GivenName:({0}* OR {0}) ", givenName.Trim());
            }

            if (familyName != null && familyName.Length < 3)
            {
                _log.LogWarning("Family Name must have a minimum of 3 characters");
                return _httpResponseMessageHelper.NoContent();
            }

            if (!string.IsNullOrEmpty(familyName))
            {
                query += string.Format("FamilyName:({0}* OR {0}) ", familyName.Trim());
            }

            var uniqueLearnerNumber = _httpRequestHelper.GetQueryString(req, "UniqueLearnerNumber");

            if (!string.IsNullOrEmpty(uniqueLearnerNumber))
            {
                query += string.Format("UniqueLearnerNumber:{0}", uniqueLearnerNumber.Trim());
            }

            var dob = _httpRequestHelper.GetQueryString(req, "DateofBirth");

            if (!string.IsNullOrEmpty(dob))
            {
                if (DateTime.TryParse(dob.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateOfBirth))
                    filter = string.Format("DateofBirth eq {0:yyyy-MM-dd}", dateOfBirth);
                else
                    return _httpResponseMessageHelper.NoContent();
            }

            if (string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(filter))
                return _httpResponseMessageHelper.NoContent();

            _log.LogInformation("Attempting to search customers");

            var customer = await _searchCustomerService.SearchCustomerAsync(indexClient, query, filter);

            _log.LogInformation("Search completed");

            return customer == null ?
                _httpResponseMessageHelper.NoContent() :
                _httpResponseMessageHelper.Ok(_jsonHelper.SerializeObjectsAndRenameIdProperty(customer, "id", "CustomerId"));
        }
    }
}