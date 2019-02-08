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
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Function
{
    public static class PostCustomerHttpTrigger
    {
        [FunctionName("POST")]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Customer Added", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Post request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [Response(HttpStatusCode = (int)422, Description = "Customer resource validation error(s)", ShowSchema = false)]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/")]HttpRequest req, ILogger log,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IValidate validate,
            [Inject]IPostCustomerHttpTriggerService customerPostService,
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


            log.LogInformation("C# HTTP trigger function Post Customer processed a request. By Touchpoint " + touchpointId);

            Models.Customer customerRequest;

            try
            {
                customerRequest = await httpRequestHelper.GetResourceFromRequest<Models.Customer>(req);
            }
            catch (JsonException ex)
            {
                return httpResponseMessageHelper.UnprocessableEntity(ex);
            }

            if (customerRequest == null)
                return httpResponseMessageHelper.UnprocessableEntity(req);

            customerRequest.LastModifiedTouchpointId = touchpointId;

            var errors = validate.ValidateResource(customerRequest,true);

            if (errors.Any())
                return httpResponseMessageHelper.UnprocessableEntity(errors);
            
            var customer = await customerPostService.CreateNewCustomerAsync(customerRequest);

            log.LogInformation("Apimurl:  " + ApimURL);

            if (customer != null)
                await customerPostService.SendToServiceBusQueueAsync(customer, ApimURL.ToString());

            return customer == null
                ? httpResponseMessageHelper.BadRequest()
                : httpResponseMessageHelper.Created(jsonHelper.SerializeObjectAndRenameIdProperty(customer, "id", "customerId"));

        }
    }
}
