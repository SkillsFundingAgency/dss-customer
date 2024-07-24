using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Function
{
    public class PatchCustomerHttpTrigger
    {
        private readonly IResourceHelper _resourceHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IValidate _validate;
        private readonly IPatchCustomerHttpTriggerService _customerPatchService;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger log; 
        private readonly IDocumentDBProvider _provider;

        public PatchCustomerHttpTrigger(IResourceHelper resourceHelper,
             IHttpRequestHelper httpRequestHelper,
             IValidate validate,
             IPatchCustomerHttpTriggerService customerPatchService,
             IJsonHelper jsonHelper,
             ILogger<PatchCustomerHttpTrigger> logger,
             IDocumentDBProvider provider)
        {
            _resourceHelper = resourceHelper;
            _httpRequestHelper = httpRequestHelper;
            _validate = validate;
            _customerPatchService = customerPatchService;
            _jsonHelper = jsonHelper;
            log = logger; 
            _provider = provider;
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
                log.LogWarning("UResponse Status Code: [{response.StatusCode}]. nable to locate 'APIM-TouchpointId' in request header");
                return response;
            }

            var ApimURL = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(ApimURL))
            {
                var response = new BadRequestObjectResult(400);
                log.LogWarning("UResponse Status Code: [{response.StatusCode}]. nable to locate 'apimurl' in request header");
                return response;
            }

            log.LogInformation($"C# HTTP trigger function Patch Customer processed a request. By Touchpoint {touchpointId}");

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                var response = new BadRequestObjectResult(customerGuid);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to parse 'customerId' to a Guid: {customerId}");
                return response;
            }

            var subContractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subContractorId))
                log.LogInformation($"Unable to locate 'SubContractorId' in request header");
            
            Models.CustomerPatch customerPatchRequest;

            try
            {
                log.LogInformation($"Attempt to get resource from body of the request");
                customerPatchRequest = await _httpRequestHelper.GetResourceFromRequest<Models.CustomerPatch>(req);
            }
            catch (JsonException ex)
            {
                var response = new UnprocessableEntityObjectResult(ex);
                log.LogError($"Response Status Code: [{response.StatusCode}]. Unable to retrieve body from req", ex);
                return response;
            }

            if (customerPatchRequest == null)
            {
                var response =new UnprocessableEntityObjectResult(req);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. customer patch request is null");
                return response;
            }

            log.LogInformation($"Attempt to set id's for action plan patch");
            customerPatchRequest.SetIds(touchpointId, subContractorId);

          
            log.LogInformation($"Attempting to see if customer exists {customerId}");
            var doesCustomerExist = await _resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                var response = new NoContentResult();
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Customer does not exist {customerGuid}");
                return response;
            }

            log.LogInformation($"Attempting to see if this is a read only customer {customerGuid}");
            var isCustomerReadOnly = await _resourceHelper.IsCustomerReadOnly(customerGuid);

            if (isCustomerReadOnly)
            {
                var response = new StatusCodeResult(403);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Customer is readonly {customerGuid}");
                return response;
            }

            log.LogInformation($"Attempting to get Customer {customerGuid}");
            var customer = await _customerPatchService.GetCustomerByIdAsync(customerGuid);

            if (customer == null)
            {
                var response = new NoContentResult();
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to get Customer resource {customerGuid}");
                return response;
            }

            dynamic data = Newtonsoft.Json.Linq.JObject.Parse(customer);
            if (data.IntroducedBy != null && customerPatchRequest.IntroducedBy == null)
                customerPatchRequest.IntroducedBy = data.IntroducedBy;


            log.LogInformation($"Attempt to validate resource");
            var errors = _validate.ValidateResource(customerPatchRequest, false);

            if (errors != null && errors.Any())
            {
                var response = new UnprocessableEntityObjectResult(errors);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. validation errors with resource", errors);
                return response;
            }


            log.LogInformation($"Attempting to patch customer resource {customerGuid}");
            var patchedCustomer = _customerPatchService.PatchResource(customer, customerPatchRequest);

            log.LogInformation($"Attempting to update Customer {customerGuid}");
            var updatedCustomer = await _customerPatchService.UpdateCosmosAsync(patchedCustomer, customerGuid);

            var di = await _provider.GetIdentityForCustomerAsync(customerGuid);
            if (di != null)
            {
                //Patches do not need to contain all the fields, only the fields that have changed, however
                //messages that are pushed onto the service bus, need to have both fields set, otherwise
                //the FamilyName/Given name are set to null in Azure B2C.
                customerPatchRequest.FamilyName = updatedCustomer.FamilyName;
                customerPatchRequest.GivenName = updatedCustomer.GivenName;

                //if customer is marked as terminated, delete di
                if (customerPatchRequest.DateOfTermination.HasValue)
                {
                    di.DateOfClosure = DateTime.Now;
                    di.LastModifiedTouchpointId = touchpointId;
                    di.ttl = 10;
                    await _provider.UpdateIdentityAsync(di);
                }

                //only interested in digitial identities that have a identitystoreid
                //e.g. ones that have had their corresponding accounts created in Azure B2C
                if (di.IdentityStoreId.HasValue)
                {
                    //mark patch request as a di account
                    customerPatchRequest.SetUpdateDigitalAccount(di.IdentityStoreId.Value);

                    var updated = await _provider.UpdateIdentityAsync(di);

                    //if digital identity was updated successfully, then mark request as a di
                    //so that it can be queued up for deletetion on azure service bus.
                    if (updated != null && customerPatchRequest.DateOfTermination.HasValue)
                    {
                        customerPatchRequest.SetDeleteDigitalIdentity();
                    }
                }

            }

            if (updatedCustomer != null)
            {
                log.LogInformation($"attempting to send to service bus {customerGuid}");
                await _customerPatchService.SendToServiceBusQueueAsync(customerPatchRequest, customerGuid, ApimURL);
            }

            if (updatedCustomer == null)
            {

                var response = new BadRequestObjectResult(400);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to update customer {customerGuid}");    
                return response;
            }
            else
            {
                var response = new OkObjectResult(_jsonHelper.SerializeObjectAndRenameIdProperty(updatedCustomer, "id", "CustomerId"));
                log.LogInformation($"Response Status Code: [{response.StatusCode}]. Update customer succeeded");
                return response;
            }

        }
    }
}


