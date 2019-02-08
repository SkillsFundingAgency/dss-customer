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
using NCS.DSS.Customer.Ioc;
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
            [Inject]IJsonHelper jsonHelper)
        {
            var touchpointId = httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                log.LogInformation("Unable to locate 'APIM-TouchpointId' in request header");
                return httpResponseMessageHelper.BadRequest();
            }

            var ApimURL = httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(ApimURL))
            {
                log.LogInformation("Unable to locate 'apimurl' in request header");
                return httpResponseMessageHelper.BadRequest();
            }

            log.LogInformation("C# HTTP trigger function Patch Customer processed a request. By Touchpoint " + touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
                return httpResponseMessageHelper.BadRequest(customerGuid);

            Models.CustomerPatch customerPatchRequest;

            try
            {
                customerPatchRequest = await httpRequestHelper.GetResourceFromRequest<Models.CustomerPatch>(req);
            }
            catch (JsonException ex)
            {
                return httpResponseMessageHelper.UnprocessableEntity(ex);
            }

            if (customerPatchRequest == null)
                return httpResponseMessageHelper.UnprocessableEntity(req);

            customerPatchRequest.LastModifiedTouchpointId = touchpointId;

            // validate the request
            var errors = validate.ValidateResource(customerPatchRequest, false);

            if (errors.Any())
                return httpResponseMessageHelper.UnprocessableEntity(errors);

            var doesCustomerExist = await resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
                return httpResponseMessageHelper.NoContent(customerGuid);

            var isCustomerReadOnly = await resourceHelper.IsCustomerReadOnly(customerGuid);

            if (isCustomerReadOnly)
                return httpResponseMessageHelper.Forbidden(customerGuid);

            var customer = await customerPatchService.GetCustomerByIdAsync(customerGuid);

            if (customer == null)
                return httpResponseMessageHelper.NoContent(customerGuid);

            var updatedCustomer = await customerPatchService.UpdateCustomerAsync(customer, customerPatchRequest);

            log.LogInformation("Apimurl:  " + ApimURL);

            if (updatedCustomer != null)
                await customerPatchService.SendToServiceBusQueueAsync(customerPatchRequest, customerGuid, ApimURL);
            
            return updatedCustomer == null ?
                httpResponseMessageHelper.BadRequest(customerGuid) :
                httpResponseMessageHelper.Ok(jsonHelper.SerializeObjectAndRenameIdProperty(updatedCustomer, "id", "customerId"));

        }
    }
}
