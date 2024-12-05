using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using System.Net;
using System.Text.Json;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Function
{
    public class PostCustomerHttpTrigger
    {
        private readonly ICosmosDBProvider _cosmosProvider;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IValidate _validate;
        private readonly IPostCustomerHttpTriggerService _customerPostService;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<PostCustomerHttpTrigger> log;
        private readonly IDynamicHelper _dynamicHelper;

        public PostCustomerHttpTrigger(ICosmosDBProvider cosmosProvider,
             IHttpRequestHelper httpRequestHelper,
             IValidate validate,
             IPostCustomerHttpTriggerService customerPostService,
             IJsonHelper jsonHelper,
             ILogger<PostCustomerHttpTrigger> logger,
             IDynamicHelper dynamicHelper
        )
        {
            _cosmosProvider = cosmosProvider;
            _httpRequestHelper = httpRequestHelper;
            _validate = validate;
            _customerPostService = customerPostService;
            _jsonHelper = jsonHelper;
            log = logger;
            _dynamicHelper = dynamicHelper;
        }

        [Function("POST")]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Customer Added", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Post request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [Response(HttpStatusCode = (int)422, Description = "Customer resource validation error(s)", ShowSchema = false)]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/")] HttpRequest req)
        {
            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                log.LogInformation("Unable to locate 'DssCorrelationId; in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                log.LogInformation("Unable to Parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            log.LogInformation("DssCorrelationId: {correlationGuid}",correlationId);

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                var response = new BadRequestObjectResult(HttpStatusCode.BadRequest);
                log.LogWarning("Response status code: {StatusCode}. Unable to locate 'TouchpointId' in request header", response.StatusCode);
                return response;
            }

            var ApimURL = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(ApimURL))
            {
                var response = new BadRequestObjectResult(HttpStatusCode.BadRequest);
                log.LogWarning("Response status code: {StatusCode}. Unable to locate 'apimurl' in request header", response.StatusCode);
                return response;
            }

            log.LogInformation("Apimurl:  " + ApimURL);

            var subContractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subContractorId))
                log.LogInformation("Unable to locate 'SubContractorId' in request header");

            log.LogInformation("C# HTTP trigger function Post Customer processed a request. By Touchpoint " + touchpointId);

            Models.Customer customerRequest;

            try
            {
                log.LogInformation("Attempt to get resource from body of the request");
                customerRequest = await _httpRequestHelper.GetResourceFromRequest<Models.Customer>(req);

            }
            catch (Exception ex)
            {
                var response = new UnprocessableEntityObjectResult(_dynamicHelper.ExcludeProperty(ex, ["TargetSite"]));
                if (ex.Message.Contains("IntroducedBy"))
                {
                    response = new UnprocessableEntityObjectResult("Please supply a valid Introduced By value.");
                    log.LogWarning("Response status code: {StatusCode}. Please supply a valid Introduced By value.", response.StatusCode);
                }
                else
                {
                    log.LogError(ex,"Response status code: {StatusCode}. JsonSerializationException error:{Error} ", response.StatusCode, ex.Message);
                }
                return response;
            }

            if (customerRequest == null)
            {
                var response = new UnprocessableEntityObjectResult(req);
                log.LogWarning("Response status code: {StatusCode}. Customer request is null", response.StatusCode);
                return response;
            }

            log.LogInformation("Attempt to set id's for action plan patch");
            customerRequest.SetIds(touchpointId, subContractorId);

            var errors = _validate.ValidateResource(customerRequest, true);

            if (errors != null && errors.Any())
            {
                var response = new UnprocessableEntityObjectResult(errors);
                log.LogWarning("Response status code: {StatusCode}. Validation errors. {errors}", response.StatusCode, errors);
                return response;
            }

            log.LogInformation("Attempt to create a new Customer");
            var customer = await _customerPostService.CreateNewCustomerAsync(customerRequest);


            if (customer != null)
            {
                log.LogInformation("Attempt to send to service bus");
                await _customerPostService.SendToServiceBusQueueAsync(customer, ApimURL.ToString());
            }
            if (customer == null)
            {
                var response = new BadRequestObjectResult(400);
                log.LogWarning("Response status code: {StatusCode}. Post a customer failed.", response.StatusCode);
                return response;
            }
            else
            {
                var response = new JsonResult(customer, new JsonSerializerOptions())
                {
                    StatusCode = (int)HttpStatusCode.Created
                };
                log.LogInformation("Response status code: {StatusCode}. Post a customer succeeded",response.StatusCode);
                return response;
            }
        }
    }
}
