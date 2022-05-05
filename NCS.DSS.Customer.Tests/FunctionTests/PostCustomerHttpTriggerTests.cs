using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Tests.FunctionTests
{
    [TestFixture]
    public class PostCustomerHttpTriggerTests
    {

        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";
        private Mock<ILogger> _log;
        private HttpRequest _request;
        private Mock<IResourceHelper> _resourceHelper;
        private IValidate _validate;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IJsonHelper _jsonHelper;
        private Mock<IPostCustomerHttpTriggerService> _postCustomerHttpTriggerService;
        private Models.Customer _customer;
        private PostCustomerHttpTrigger.Function.PostCustomerHttpTrigger _function;

        [SetUp]
        public void Setup()
        {
            _customer = new Models.Customer();
            _request = new DefaultHttpRequest(new DefaultHttpContext());

            _log = new Mock<ILogger>();
            _resourceHelper = new Mock<IResourceHelper>();
            _validate = new Validate();
            _loggerHelper = new Mock<ILoggerHelper>();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _jsonHelper = new JsonHelper();
            _postCustomerHttpTriggerService = new Mock<IPostCustomerHttpTriggerService>();
            _function = new PostCustomerHttpTrigger.Function.PostCustomerHttpTrigger(_resourceHelper.Object,
                _httpRequestHelper.Object,
                _httpResponseMessageHelper,
                _validate,
                _postCustomerHttpTriggerService.Object,
                _jsonHelper,
                _loggerHelper.Object);
        }

        [Test]
        public async Task PostCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // Arrange
            _httpRequestHelper.Setup(x=>x.GetDssTouchpointId(_request)).Returns((string)null);

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PostCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenSubcontractorIdIsNotProvided()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns((string)null);

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PostCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerHasFailedValidation()
        {
            // Arrange
            _httpRequestHelper.Setup(x=>x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x=>x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<Models.Customer>(_request)).Returns(Task.FromResult(_customer));
            var validationResults = new List<ValidationResult> { new ValidationResult("Customer Id is Required") };
            var val = new Mock<IValidate>();
            val.Setup(x=>x.ValidateResource(It.IsAny<Models.Customer>(), It.IsAny<bool>())).Returns(validationResults);
            _function = new PostCustomerHttpTrigger.Function.PostCustomerHttpTrigger(_resourceHelper.Object,
                _httpRequestHelper.Object,
                _httpResponseMessageHelper,
                val.Object,
                _postCustomerHttpTriggerService.Object,
                _jsonHelper,
                _loggerHelper.Object);

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PostCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerRequestIsInvalid()
        {
            // Arrange
            _postCustomerHttpTriggerService.Setup(x => x.CreateNewCustomerAsync(It.IsAny<Models.Customer>())).Returns(Task.FromResult(_customer));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x=>x.GetResourceFromRequest<Models.Customer>(It.IsAny<HttpRequest>())).ThrowsAsync(new JsonException());

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PostCustomerHttpTrigger_ReturnsStatusBadRequest_WhenRequestIsNotValid()
        {
            // Arrange
            _postCustomerHttpTriggerService.Setup(x=>x.CreateNewCustomerAsync(It.IsAny<Models.Customer>())).Returns(Task.FromResult<Models.Customer>(null));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<Models.Customer>(_request)).Returns(Task.FromResult(_customer));
            var validationResults = new List<ValidationResult>();
            var val = new Mock<IValidate>();
            val.Setup(x => x.ValidateResource(It.IsAny<Models.Customer>(), It.IsAny<bool>())).Returns(validationResults);
            _function = new PostCustomerHttpTrigger.Function.PostCustomerHttpTrigger(_resourceHelper.Object,
                _httpRequestHelper.Object,
                _httpResponseMessageHelper,
                val.Object,
                _postCustomerHttpTriggerService.Object,
                _jsonHelper,
                _loggerHelper.Object);

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PostCustomerHttpTrigger_ReturnsStatusCodeCreated_WhenRequestIsValid()
        {
            // Arrange
            _postCustomerHttpTriggerService.Setup(x=>x.CreateNewCustomerAsync(It.IsAny<Models.Customer>())).Returns(Task.FromResult(_customer));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<Models.Customer>(_request)).Returns(Task.FromResult(_customer));
            var validationResults = new List<ValidationResult>();
            var val = new Mock<IValidate>();
            val.Setup(x => x.ValidateResource(It.IsAny<Models.Customer>(), It.IsAny<bool>())).Returns(validationResults);
            _function = new PostCustomerHttpTrigger.Function.PostCustomerHttpTrigger(_resourceHelper.Object,
                _httpRequestHelper.Object,
                _httpResponseMessageHelper,
                val.Object,
                _postCustomerHttpTriggerService.Object,
                _jsonHelper,
                _loggerHelper.Object);

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
        }

        private async Task<HttpResponseMessage> RunFunction(string customerId)
        {
            return await _function.RunAsync(
                _request,
                _log.Object).ConfigureAwait(false);
        }
    }
}
