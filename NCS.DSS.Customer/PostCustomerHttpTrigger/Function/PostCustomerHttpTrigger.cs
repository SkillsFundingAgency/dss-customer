using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Annotations;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Ioc;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Function
{
    public static class PostCustomerHttpTrigger
    {
        [FunctionName("POST")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Customer Added", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Post request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [Response(HttpStatusCode = (int)422, Description = "Customer resource validation error(s)", ShowSchema = false)]
        [ResponseType(typeof(Models.Customer))]
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/")]HttpRequestMessage req, ILogger log,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IHttpRequestMessageHelper httpRequestMessageHelper,
            [Inject]IValidate validate,
            [Inject]IPostCustomerHttpTriggerService customerPostService)
        {
            var touchpointId = httpRequestMessageHelper.GetTouchpointId(req);
            if (touchpointId == null)
            {
                log.LogInformation("Unable to locate 'APIM-TouchpointId' in request header");
                return HttpResponseMessageHelper.BadRequest();
            }

            log.LogInformation("C# HTTP trigger function Post Customer processed a request. By Touchpoint " + touchpointId);

            Models.Customer customerRequest;

            try
            {
                customerRequest = await httpRequestMessageHelper.GetCustomerFromRequest<Models.Customer>(req);
            }
            catch (JsonException ex)
            {
                return HttpResponseMessageHelper.UnprocessableEntity(ex);
            }

            if (customerRequest == null)
                return HttpResponseMessageHelper.UnprocessableEntity(req);

            customerRequest.LastModifiedTouchpointId = touchpointId;

            var errors = validate.ValidateResource(customerRequest);

            if (errors.Any())
                return HttpResponseMessageHelper.UnprocessableEntity(errors);
            
            var customer = await customerPostService.CreateNewCustomerAsync(customerRequest);

            return customer == null
                ? HttpResponseMessageHelper.BadRequest()
                : HttpResponseMessageHelper.Created(customer);

        }
    }
}
