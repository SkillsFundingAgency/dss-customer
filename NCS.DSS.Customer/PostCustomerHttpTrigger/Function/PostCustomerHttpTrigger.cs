using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.Customer;
using System.Web.Http.Description;
using System;
using NCS.DSS.Customer.Annotations;
using NCS.DSS.Customer.ReferenceData;
using System.Dynamic;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using Newtonsoft.Json.Linq;
using NCS.DSS.Customer.Ioc;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Validation;
using System.Linq;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger
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
        public static async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/")]HttpRequestMessage req, TraceWriter log,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IHttpRequestMessageHelper httpRequestMessageHelper,
            [Inject]IValidate validate,
            [Inject]IPostCustomerHttpTriggerService customerPostService)
        {

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

            var errors = validate.ValidateResource(customerRequest);

            if (errors != null && errors.Any())
                return HttpResponseMessageHelper.UnprocessableEntity(errors);
            
            var customer = await customerPostService.CreateNewCustomerAsync(customerRequest);

            return customer == null
                ? HttpResponseMessageHelper.BadRequest()
                : HttpResponseMessageHelper.Created(customer);

        }
    }
}
