using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;
using NCS.DSS.Customer.Annotations;
using System;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Ioc;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Validation;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using System.Linq;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger
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
        [ResponseType(typeof(Models.Customer))]
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "patch", 
            Route = "Customers/{customerId}")]HttpRequestMessage req, TraceWriter log, string customerId,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IHttpRequestMessageHelper httpRequestMessageHelper,
            [Inject]IValidate validate,
            [Inject]IPatchCustomerHttpTriggerService customerPatchService)
        {
            if (!Guid.TryParse(customerId, out var customerGuid))
                return HttpResponseMessageHelper.BadRequest(customerGuid);

            var customerPatch = await httpRequestMessageHelper.GetCustomerFromRequest<Models.CustomerPatch>(req);
            customerPatch.CustomerID = customerGuid;

            if(customerPatch == null)
                return HttpResponseMessageHelper.UnprocessableEntity(req);

            // validate the request
            var errors = validate.ValidateResource(customerPatch);

            if (errors != null && errors.Any())
                return HttpResponseMessageHelper.UnprocessableEntity("Validation error(s) : ", errors);

            var doesCustomerExist = resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
                return HttpResponseMessageHelper.NoContent("Unable to find a customer with Id of : ", customerGuid);

            var customer = await customerPatchService.GetCustomerByIdAsync(customerGuid);
            var updatedCustomer = await customerPatchService.UpdateCustomerAsync(customer, customerPatch);
            
            return updatedCustomer == null ?
                HttpResponseMessageHelper.BadRequest("Unable to find update customer with Id of : ", customerGuid) :
                HttpResponseMessageHelper.Ok(updatedCustomer);

        }
    }
}
