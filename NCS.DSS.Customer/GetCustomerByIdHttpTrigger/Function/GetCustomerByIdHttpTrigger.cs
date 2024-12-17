using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using System.Net;
using System.Text.Json;

namespace NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Function
{
    public class GetCustomerByIdHttpTrigger
    {
        private readonly ICosmosDBProvider _cosmosProvider;
        private readonly IGetCustomerByIdHttpTriggerService _customerByIdService;
        private readonly ILogger<GetCustomerByIdHttpTrigger> _logger;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IJsonHelper _jsonHelper;

        public GetCustomerByIdHttpTrigger(ICosmosDBProvider cosmosProvider,
            IGetCustomerByIdHttpTriggerService customerByIdService,
            ILogger<GetCustomerByIdHttpTrigger> logger,
            IHttpRequestHelper httpRequestHelper,
            IJsonHelper jsonHelper)
        {
            _cosmosProvider = cosmosProvider;
            _customerByIdService = customerByIdService;
            _logger = logger;
            _httpRequestHelper = httpRequestHelper;
            _jsonHelper = jsonHelper;
        }

        [Function("GETByID")]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Customer does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}")] HttpRequest req, string customerId)
        {
            var functionName = nameof(GetCustomerByIdHttpTrigger);

            _logger.LogInformation("Function {FunctionName} has been invoked", functionName);

            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                _logger.LogWarning("Unable to locate 'DssCorrelationId; in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                _logger.LogWarning("Unable to Parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            _logger.LogInformation("DssCorrelationId: [{correlationGuid}]", correlationGuid);

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                _logger.LogError("{CorrelationID} Unable to locate 'APIM-TouchpointId' in request header", correlationGuid);
                return new BadRequestObjectResult(HttpStatusCode.BadRequest);;
            }

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                _logger.LogError("{CorrelationID} Unable to parse 'customerId' to a Guid: {customerId}", correlationGuid, customerId);
                return new BadRequestObjectResult(customerGuid);;
            }

            _logger.LogInformation("{CorrelationID} Attempting to get customer {customerId}",correlationGuid, customerId);
            var customer = await _customerByIdService.GetCustomerAsync(customerGuid);

            if (customer == null)
            {
                _logger.LogError("{CorrelationID} Customer not found {customerId}", correlationGuid, customerId);
                return new NoContentResult();;
            }

            _logger.LogInformation("Function {FunctionName} has finished invoking", functionName);
            var response = new JsonResult(customer, new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            _logger.LogInformation("Response Status Code: {StatusCode}. Get customer succeeded", response.StatusCode);
            return response;

        }
    }
}