using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using System.Net;
using System.Text.Json;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Function
{
    public class PatchCustomerHttpTrigger
    {
        private readonly ICosmosDBProvider _cosmosProvider;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IValidate _validate;
        private readonly IPatchCustomerHttpTriggerService _customerPatchService;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<PatchCustomerHttpTrigger> log;
        private readonly ICosmosDBProvider _provider;
        private IDynamicHelper _dynamicHelper;

        public PatchCustomerHttpTrigger(ICosmosDBProvider cosmosProvider,
             IHttpRequestHelper httpRequestHelper,
             IValidate validate,
             IPatchCustomerHttpTriggerService customerPatchService,
             IJsonHelper jsonHelper,
             ILogger<PatchCustomerHttpTrigger> logger,
             ICosmosDBProvider provider,
             IDynamicHelper dynamicHelper)
        {
            _cosmosProvider = cosmosProvider;
            _httpRequestHelper = httpRequestHelper;
            _validate = validate;
            _customerPatchService = customerPatchService;
            _jsonHelper = jsonHelper;
            log = logger;
            _provider = provider;
            _dynamicHelper = dynamicHelper;
        }

        [Function("PATCH")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Patched", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Patch request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [Response(HttpStatusCode = (int)422, Description = "Customer resource validation error(s)", ShowSchema = false)]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "patch",
            Route = "Customers/{customerId}")]HttpRequest req, string customerId)
        {

            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                log.LogInformation("Unable to Parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }
            log.LogTrace("DssCorrelationId: {correlationGuid}",correlationGuid);

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                var response = new BadRequestObjectResult(HttpStatusCode.BadRequest);
                log.LogInformation("Response Status Code: {StatusCode}. Unable to locate 'APIM-TouchpointId' in request header", response.StatusCode);
                return response;
            }

            var ApimURL = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(ApimURL))
            {
                var response = new BadRequestObjectResult(HttpStatusCode.BadRequest);
                log.LogInformation("Response Status Code: {StatusCode}. Unable to locate 'apimurl' in request header", response.StatusCode);
                return response;
            }

            log.LogTrace("C# HTTP trigger function Patch Customer processed a request. By Touchpoint {touchpointId}",touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                var response = new BadRequestObjectResult(customerGuid);
                log.LogInformation("Response Status Code: {StatusCode}. Unable to parse 'customerId' to a Guid: {customerId}", response.StatusCode,customerId);
                return response;
            }

            var subContractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subContractorId))
                log.LogInformation("Unable to locate 'SubContractorId' in request header");

            Models.CustomerPatch customerPatchRequest;

            try
            {
                log.LogTrace("Attempt to get resource from body of the request");
                customerPatchRequest = await _httpRequestHelper.GetResourceFromRequest<Models.CustomerPatch>(req);
            }
            catch
            {
                var response = new UnprocessableEntityObjectResult(req);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to retrieve the body from the request");
                return response;
            }

            if (customerPatchRequest == null)
            {
                var response = new UnprocessableEntityObjectResult(req);
                log.LogInformation("Response Status Code: {StatusCode}. customer patch request is null", response.StatusCode);
                return response;
            }

            log.LogTrace("Attempt to set id's for action plan patch");
            customerPatchRequest.SetIds(touchpointId, subContractorId);


            log.LogTrace("Attempting to see if customer exists {customerId}",customerId);
            var doesCustomerExist = await _cosmosProvider.DoesCustomerResourceExist(customerGuid);

            if (!doesCustomerExist)
            {
                var response = new NoContentResult();
                log.LogInformation("Response Status Code: {StatusCode}. Customer does not exist {customerGuid}", response.StatusCode,customerGuid);
                return response;
            }

            log.LogTrace("Attempting to see if this is a read only customer {customerGuid}",customerGuid);
            var isCustomerReadOnly = await _cosmosProvider.DoesCustomerHaveATerminationDate(customerGuid);

            if (isCustomerReadOnly)
            {
                var response = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                log.LogInformation("Response Status Code: {StatusCode}. Customer is readonly {customerGuid}", response.StatusCode,customerGuid);
                return response;
            }

            log.LogTrace("Attempting to get Customer {customerGuid}",customerGuid);
            var customer = await _customerPatchService.GetCustomerByIdAsync(customerGuid);

            if (customer == null)
            {
                var response = new NoContentResult();
                log.LogInformation("Response Status Code: {StatusCode}. Unable to get Customer resource {customerGuid}", response.StatusCode,customerGuid);
                return response;
            }

            dynamic data = Newtonsoft.Json.Linq.JObject.Parse(customer);
            if (data.IntroducedBy != null && customerPatchRequest.IntroducedBy == null)
                customerPatchRequest.IntroducedBy = data.IntroducedBy;


            log.LogTrace("Attempt to validate resource");
            var errors = _validate.ValidateResource(customerPatchRequest);

            if (errors != null && errors.Any())
            {
                var response = new UnprocessableEntityObjectResult(errors);
                log.LogWarning("Response Status Code: {StatusCode}. validation errors with resource {errors}", response.StatusCode, errors);
                return response;
            }


            log.LogTrace("Attempting to patch customer resource {customerGuid}",customerGuid);
            var patchedCustomer = _customerPatchService.PatchResource(customer, customerPatchRequest);

            log.LogTrace("Attempting to update Customer {customerGuid}",customerGuid);
            var updatedCustomer = await _customerPatchService.UpdateCosmosAsync(patchedCustomer, customerGuid);

            if (updatedCustomer != null)
            {
                log.LogTrace("attempting to send to service bus {customerGuid}",customerGuid);
                await _customerPatchService.SendToServiceBusQueueAsync(customerPatchRequest, customerGuid, ApimURL);
            }

            if (updatedCustomer == null)
            {

                var response = new BadRequestObjectResult(400);
                log.LogWarning("Response Status Code: {StatusCode}. Unable to update customer {customerGuid}", response.StatusCode,customerGuid);
                return response;
            }
            else
            {
                var response = new JsonResult(updatedCustomer, new JsonSerializerOptions())
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                log.LogTrace("Response Status Code: {StatusCode}. Update customer succeeded",response.StatusCode);
                return response;
            }

        }
    }
}


