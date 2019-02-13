using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Annotations;
using NCS.DSS.Customer.Cosmos.Helper;
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

            loggerHelper.LogInformationMessage(log, correlationGuid, "C# HTTP trigger function Patch Customer processed a request. By Touchpoint " + touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to parse customerId to Guid");
                return httpResponseMessageHelper.BadRequest(customerGuid);
            }

            var subContractorId = httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subContractorId))
                loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'SubContractorId' in request header");


            Models.CustomerPatch customerPatchRequest;

            try
            {
                customerPatchRequest = await httpRequestHelper.GetResourceFromRequest<Models.CustomerPatch>(req);
            }
            catch (JsonException ex)
            {
                loggerHelper.LogError(log, correlationGuid, "Unable to retrieve body from req", ex);
                return httpResponseMessageHelper.UnprocessableEntity(ex);
            }

            if (customerPatchRequest == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, "Customer patch request is null");
                return httpResponseMessageHelper.UnprocessableEntity(req);
            }

            customerPatchRequest.SetIds(touchpointId, subContractorId);            

            // validate the request
            var errors = validate.ValidateResource(customerPatchRequest, false);

            if (errors.Any())
                return httpResponseMessageHelper.UnprocessableEntity(errors);

            var doesCustomerExist = await resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer do not exist {0}", customerGuid));
                return httpResponseMessageHelper.NoContent(customerGuid);
            }

            var isCustomerReadOnly = await resourceHelper.IsCustomerReadOnly(customerGuid);

            if (isCustomerReadOnly)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer is readonly {0}", customerGuid));
                return httpResponseMessageHelper.Forbidden(customerGuid);
            }

            var customer = await customerPatchService.GetCustomerByIdAsync(customerGuid);

            if (customer == null)
                return httpResponseMessageHelper.NoContent(customerGuid);

            var patchedCustomer = customerPatchService.PatchResource(customer, customerPatchRequest);

            var updatedCustomer = await customerPatchService.UpdateCosmosAsync(patchedCustomer);

            loggerHelper.LogInformationMessage(log, correlationGuid, "Apimurl:  " + ApimURL);

            if (updatedCustomer != null)
                await customerPatchService.SendToServiceBusQueueAsync(customerPatchRequest, customerGuid, ApimURL);

            loggerHelper.LogMethodExit(log);

            return updatedCustomer == null ?
                httpResponseMessageHelper.BadRequest(customerGuid) :
                httpResponseMessageHelper.Ok(jsonHelper.SerializeObjectAndRenameIdProperty(updatedCustomer, "id", "customerId"));

        }
    }
}
