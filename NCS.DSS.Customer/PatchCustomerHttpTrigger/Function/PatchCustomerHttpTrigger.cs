using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.Swagger.Standard.Annotations;
using Newtonsoft.Json.Linq;
using NCS.DSS.Customer.Helpers;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Function
{
    public static class PatchCustomerHttpTrigger
    {
        [FunctionName("PATCH")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Patched", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Patch request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [Response(HttpStatusCode = (int)422, Description = "Customer resource validation error(s)", ShowSchema = false)]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "patch", 
            Route = "Customers/{customerId}")]HttpRequest req, ILogger log, string customerId,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IValidate validate,
            [Inject]IPatchCustomerHttpTriggerService customerPatchService,
            [Inject]IJsonHelper jsonHelper,
            [Inject]ILoggerHelper loggerHelper)
        {

            loggerHelper.LogMethodEnter(log);

            var correlationId = httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                log.LogInformation("Unable to locate 'DssCorrelationId; in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                log.LogInformation("Unable to Parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            var touchpointId = httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'APIM-TouchpointId' in request header");
                return httpResponseMessageHelper.BadRequest();
            }

            var ApimURL = httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(ApimURL))
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'apimurl' in request header");
                return httpResponseMessageHelper.BadRequest();
            }

            loggerHelper.LogInformationMessage(log, correlationGuid,
                "C# HTTP trigger function Patch Customer processed a request. By Touchpoint " + touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'customerId' to a Guid: {0}", customerId));
                return httpResponseMessageHelper.BadRequest(customerGuid);
            }

            var subContractorId = httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subContractorId))
                loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'SubContractorId' in request header");
            
            Models.CustomerPatch customerPatchRequest;

            try
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to get resource from body of the request");
                customerPatchRequest = await httpRequestHelper.GetResourceFromRequest<Models.CustomerPatch>(req);
            }
            catch (JsonException ex)
            {
                loggerHelper.LogError(log, correlationGuid, "Unable to retrieve body from req", ex);
                return httpResponseMessageHelper.UnprocessableEntity(ex);
            }

            if (customerPatchRequest == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, "customer patch request is null");
                return httpResponseMessageHelper.UnprocessableEntity(req);
            }

            loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to set id's for action plan patch");
            customerPatchRequest.SetIds(touchpointId, subContractorId);

            loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to validate resource");
            var errors = validate.ValidateResource(customerPatchRequest, false);

            if (errors != null && errors.Any())
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, "validation errors with resource");
                return httpResponseMessageHelper.UnprocessableEntity(errors);
            }

            loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if customer exists {0}", customerGuid));
            var doesCustomerExist = await resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer does not exist {0}", customerGuid));
                return httpResponseMessageHelper.NoContent(customerGuid);
            }

            loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if this is a read only customer {0}", customerGuid));
            var isCustomerReadOnly = await resourceHelper.IsCustomerReadOnly(customerGuid);

            if (isCustomerReadOnly)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer is readonly {0}", customerGuid));
                return httpResponseMessageHelper.Forbidden(customerGuid);
            }

            loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get Customer {0}", customerGuid));
            var customer = await customerPatchService.GetCustomerByIdAsync(customerGuid);

            if (customer == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to get Customer resource {0}", customerGuid));
                return httpResponseMessageHelper.NoContent(customerGuid);
            }

            loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to patch customer resource {0}", customerGuid));
            var patchedCustomer = customerPatchService.PatchResource(customer, customerPatchRequest);

            loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to update Customer {0}", customerGuid));
            var updatedCustomer = await customerPatchService.UpdateCosmosAsync(patchedCustomer, customerGuid);

            if (updatedCustomer != null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("attempting to send to service bus {0}", customerGuid));
                await customerPatchService.SendToServiceBusQueueAsync(customerPatchRequest, customerGuid, ApimURL);
            }

            loggerHelper.LogMethodExit(log);

            return updatedCustomer == null ?
                httpResponseMessageHelper.BadRequest(customerGuid) :
                httpResponseMessageHelper.Ok(jsonHelper.SerializeObjectAndRenameIdProperty(updatedCustomer, "id", "CustomerId"));

        }
    }
}
