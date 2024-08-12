using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Function
{
    public class GetCustomerByIdHttpTrigger
    {
        private readonly IResourceHelper _resourceHelper;
        private readonly IGetCustomerByIdHttpTriggerService _customerByIdService;
        private readonly ILogger log;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IJsonHelper _jsonHelper;

        public GetCustomerByIdHttpTrigger(IResourceHelper resourceHelper,
            IGetCustomerByIdHttpTriggerService customerByIdService,
            ILogger<GetCustomerByIdHttpTrigger> logger,
            IHttpRequestHelper httpRequestHelper,
            IJsonHelper jsonHelper)
        {
            _resourceHelper = resourceHelper;
            _customerByIdService = customerByIdService;
            log = logger;
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

            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                log.LogInformation("Unable to locate 'DssCorrelationId; in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                log.LogInformation("Unable to Parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

             log.LogInformation($"DssCorrelationId: [{correlationGuid}]");

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                var response = new BadRequestObjectResult(400);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to locate 'APIM-TouchpointId' in request header");
                return response;
            }

            log.LogInformation($"C# HTTP trigger function GetCustomerById processed a request. By Touchpoint " + touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                var response = new BadRequestObjectResult(customerGuid);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to parse 'customerId' to a Guid: {customerId}");
                return response;
            }

            log.LogInformation($"Attempting to get customer {customerId}");
            var customer = await _customerByIdService.GetCustomerAsync(customerGuid);


            if (customer == null)
            {
                var response = new NoContentResult();
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Customer not found {customerId}");
                return response;
            }
            else
            {
                
                var response = new JsonResult(customer, new JsonSerializerOptions())
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                log.LogInformation($"Response Status Code: [{response.StatusCode}]. Get customer succeeded");
                return response;
            }
        }
    }
}