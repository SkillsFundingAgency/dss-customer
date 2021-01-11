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
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Function
{
    public class GetCustomerByIdHttpTrigger
    {
        private readonly IResourceHelper _resourceHelper;
        private readonly IGetCustomerByIdHttpTriggerService _customerByIdService;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IHttpResponseMessageHelper _httpResponseMessageHelper;
        private readonly IJsonHelper _jsonHelper;

        public GetCustomerByIdHttpTrigger(IResourceHelper resourceHelper,
            IGetCustomerByIdHttpTriggerService customerByIdService,
            ILoggerHelper loggerHelper,
            IHttpRequestHelper httpRequestHelper,
            IHttpResponseMessageHelper httpResponseMessageHelper,
            IJsonHelper jsonHelper)
        {
            _resourceHelper = resourceHelper;
            _customerByIdService = customerByIdService;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
            _httpResponseMessageHelper = httpResponseMessageHelper;
            _jsonHelper = jsonHelper;
        }

        [FunctionName("GETByID")]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Customer does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        public async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}")] HttpRequest req, ILogger log, string customerId)
        {
            _loggerHelper.LogMethodEnter(log);

            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                log.LogInformation("Unable to locate 'DssCorrelationId; in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                log.LogInformation("Unable to Parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'APIM-TouchpointId' in request header");
                return _httpResponseMessageHelper.BadRequest();
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid,
                "C# HTTP trigger function GetCustomerById processed a request. By Touchpoint " + touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'customerId' to a Guid: {0}", customerId));
                return _httpResponseMessageHelper.BadRequest(customerGuid);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get  customer {0}", customerGuid));
            var customer = await _customerByIdService.GetCustomerAsync(customerGuid);

            _loggerHelper.LogMethodExit(log);

            return customer == null ?
                _httpResponseMessageHelper.NoContent(customerGuid) :
                _httpResponseMessageHelper.Ok(_jsonHelper.SerializeObjectAndRenameIdProperty(customer, "id", "CustomerId"));
        }
    }
}