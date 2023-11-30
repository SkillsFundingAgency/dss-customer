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
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.ReferenceData;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Function
{
    public class PostCustomerHttpTrigger
    {
        private readonly IResourceHelper _resourceHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IHttpResponseMessageHelper _httpResponseMessageHelper;
        private readonly IValidate _validate;
        private readonly IPostCustomerHttpTriggerService _customerPostService;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILoggerHelper _loggerHelper;

        public PostCustomerHttpTrigger(IResourceHelper resourceHelper,
             IHttpRequestHelper httpRequestHelper,
             IHttpResponseMessageHelper httpResponseMessageHelper,
             IValidate validate,
             IPostCustomerHttpTriggerService customerPostService,
             IJsonHelper jsonHelper,
             ILoggerHelper loggerHelper
        )
        {
            _resourceHelper = resourceHelper;
            _httpRequestHelper = httpRequestHelper;
            _httpResponseMessageHelper = httpResponseMessageHelper;
            _validate = validate;
            _customerPostService = customerPostService;
            _jsonHelper = jsonHelper;
            _loggerHelper = loggerHelper;
        }

        [FunctionName("POST")]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Customer Added", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Resource Does Not Exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Post request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API Key unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient Access To This Resource", ShowSchema = false)]
        [Response(HttpStatusCode = (int)422, Description = "Customer resource validation error(s)", ShowSchema = false)]
        [ProducesResponseType(typeof(Models.Customer), 200)]
        public async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/")] HttpRequest req, ILogger log)
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
                var response =  _httpResponseMessageHelper.BadRequest();
                log.LogWarning($"Response status code: [{response.StatusCode}]. Unable to locate 'APIM-TouchpointId' in request header");
                return response;
            }

            var ApimURL = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(ApimURL))
            {
                var response = _httpResponseMessageHelper.BadRequest();
                log.LogWarning($"Response status code: [{response.StatusCode}]. Unable to locate 'apimurl' in request header");
                return response;
            }

            log.LogInformation($"Apimurl:  " + ApimURL);

            var subContractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subContractorId))
                log.LogInformation($"Unable to locate 'SubContractorId' in request header");

            log.LogInformation("C# HTTP trigger function Post Customer processed a request. By Touchpoint " + touchpointId);

            Models.Customer customerRequest;

            try
            {
                log.LogInformation($"Attempt to get resource from body of the request");
                customerRequest = await _httpRequestHelper.GetResourceFromRequest<Models.Customer>(req);
                
            }
            catch (JsonSerializationException ex)
            {
                var response = _httpResponseMessageHelper.UnprocessableEntity(ex);
                if (ex.Message.Contains("IntroducedBy"))
                {
                    log.LogWarning($"Response status code: [{response.StatusCode}]. Please supply a valid Introduced By valuel");
                }
                else
                {
                    log.LogError($"Response status code: [{response.StatusCode}]. JsonSerializationException error: ", ex.Message);
                }
                return response;
            }
            catch (JsonException ex)
            {
                var response =  _httpResponseMessageHelper.UnprocessableEntity(ex);
                log.LogError($"Response status code: [{response.StatusCode}]. Unable to retrieve body from req", ex);
                return response;
            }

            if (customerRequest == null)
            {
                var response =  _httpResponseMessageHelper.UnprocessableEntity(req);
                log.LogWarning($"Response status code: [{response.StatusCode}]. Customer request is null");
                return response;
            }

            log.LogInformation($"Attempt to set id's for action plan patch");
            customerRequest.SetIds(touchpointId, subContractorId);

            var errors = _validate.ValidateResource(customerRequest, true);

            if (errors != null && errors.Any())
            {
                var response =  _httpResponseMessageHelper.UnprocessableEntity(errors);
                log.LogWarning($"Response status code: [{response.StatusCode}]. Validation errors.", errors);
                return response;
            }

            log.LogInformation($"Attempt to create a new Customer");
            var customer = await _customerPostService.CreateNewCustomerAsync(customerRequest);


            if (customer != null)
            {
                log.LogInformation($"Attempt to send to service bus");
                //await _customerPostService.SendToServiceBusQueueAsync(customer, ApimURL.ToString());
            }
            if (customer == null)
            {
                var response =  _httpResponseMessageHelper.BadRequest();
                log.LogWarning($"Response status code: [{response.StatusCode}]. Post a customer failed.");
                return response;
            }
            else
            {
                var response =  _httpResponseMessageHelper.Created(_jsonHelper.SerializeObjectAndRenameIdProperty(customer, "id", "CustomerId"));
                log.LogInformation($"Response status code: [{response.StatusCode}]. Post a customer succeeded");
                return response;
            }
        }
    }
}
