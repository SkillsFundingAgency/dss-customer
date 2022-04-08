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
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Function
{
    public class PatchCustomerHttpTrigger
    {
        private readonly IResourceHelper _resourceHelper;
        private readonly IHttpResponseMessageHelper _httpResponseMessageHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IValidate _validate;
        private readonly IPatchCustomerHttpTriggerService _customerPatchService;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IDocumentDBProvider _provider;

        public PatchCustomerHttpTrigger(IResourceHelper resourceHelper,
             IHttpResponseMessageHelper httpResponseMessageHelper,
             IHttpRequestHelper httpRequestHelper,
             IValidate validate,
             IPatchCustomerHttpTriggerService customerPatchService,
             IJsonHelper jsonHelper,
             ILoggerHelper loggerHelper,
             IDocumentDBProvider provider)
        {
            _resourceHelper = resourceHelper;
            _httpResponseMessageHelper = httpResponseMessageHelper;
            _httpRequestHelper = httpRequestHelper;
            _validate = validate;
            _customerPatchService = customerPatchService;
            _jsonHelper = jsonHelper;
            _loggerHelper = loggerHelper;
            _provider = provider;
        }

        [FunctionName("PATCH")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Patched", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Patch request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [Response(HttpStatusCode = (int)422, Description = "Customer resource validation error(s)", ShowSchema = false)]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        public async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "patch", 
            Route = "Customers/{customerId}")]HttpRequest req, ILogger log, string customerId)
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

            var ApimURL = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(ApimURL))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'apimurl' in request header");
                return _httpResponseMessageHelper.BadRequest();
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid,
                "C# HTTP trigger function Patch Customer processed a request. By Touchpoint " + touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'customerId' to a Guid: {0}", customerId));
                return _httpResponseMessageHelper.BadRequest(customerGuid);
            }

            var subContractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subContractorId))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'APIM-subContractorId' in request header");
                return _httpResponseMessageHelper.BadRequest();
            }

            Models.CustomerPatch customerPatchRequest;

            try
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to get resource from body of the request");
                customerPatchRequest = await _httpRequestHelper.GetResourceFromRequest<Models.CustomerPatch>(req);
            }
            catch (JsonException ex)
            {
                _loggerHelper.LogError(log, correlationGuid, "Unable to retrieve body from req", ex);
                return _httpResponseMessageHelper.UnprocessableEntity(ex);
            }

            if (customerPatchRequest == null)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "customer patch request is null");
                return _httpResponseMessageHelper.UnprocessableEntity(req);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to set id's for action plan patch");
            customerPatchRequest.SetIds(touchpointId, subContractorId);

            _loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to validate resource");
            var errors = _validate.ValidateResource(customerPatchRequest, false);

            if (errors != null && errors.Any())
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "validation errors with resource");
                return _httpResponseMessageHelper.UnprocessableEntity(errors);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if customer exists {0}", customerGuid));
            var doesCustomerExist = await _resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer does not exist {0}", customerGuid));
                return _httpResponseMessageHelper.NoContent(customerGuid);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if this is a read only customer {0}", customerGuid));
            var isCustomerReadOnly = await _resourceHelper.IsCustomerReadOnly(customerGuid);

            if (isCustomerReadOnly)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer is readonly {0}", customerGuid));
                return _httpResponseMessageHelper.Forbidden(customerGuid);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get Customer {0}", customerGuid));
            var customer = await _customerPatchService.GetCustomerByIdAsync(customerGuid);

            if (customer == null)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to get Customer resource {0}", customerGuid));
                return _httpResponseMessageHelper.NoContent(customerGuid);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to patch customer resource {0}", customerGuid));
            var patchedCustomer = _customerPatchService.PatchResource(customer, customerPatchRequest);

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to update Customer {0}", customerGuid));
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
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("attempting to send to service bus {0}", customerGuid));
                await _customerPatchService.SendToServiceBusQueueAsync(customerPatchRequest, customerGuid, ApimURL);
            }

            _loggerHelper.LogMethodExit(log);

            return updatedCustomer == null ?
                _httpResponseMessageHelper.BadRequest(customerGuid) :
                _httpResponseMessageHelper.Ok(_jsonHelper.SerializeObjectAndRenameIdProperty(updatedCustomer, "id", "CustomerId"));

        }
    }
}
