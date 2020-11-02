using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Tests.FunctionTests
{
    [TestFixture]
    public class PatchCustomerHttpTriggerTests
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
        private Mock<IPatchCustomerHttpTriggerService> _patchCustomerHttpTriggerService;
        private Models.Customer _customer;
        private CustomerPatch _customerPatch;
        private string _customerString;
        private PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger _function;

        [SetUp]
        public void Setup()
        {
            _customer = new Models.Customer();
            _customerPatch = new CustomerPatch();
            _request = new DefaultHttpRequest(new DefaultHttpContext());

            _log = new Mock<ILogger>();
            _resourceHelper = new Mock<IResourceHelper>();
            _validate = new Validate();
            _loggerHelper = new Mock<ILoggerHelper>();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _jsonHelper = new JsonHelper();
            _patchCustomerHttpTriggerService = new Mock<IPatchCustomerHttpTriggerService>();
            _customerString = JsonConvert.SerializeObject(_customer);
            _function = new PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger(
                _resourceHelper.Object, 
                _httpResponseMessageHelper,
                _httpRequestHelper.Object, 
                _validate, 
                _patchCustomerHttpTriggerService.Object, 
                _jsonHelper, 
                _loggerHelper.Object);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
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
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x=>x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x=>x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerHasFailedValidation()
        {
            // Arrange
            var validationResults = new List<ValidationResult> { new ValidationResult("Customer Id is Required") };
            var val = new Mock<IValidate>();
            val.Setup(x=>x.ValidateResource(It.IsAny<CustomerPatch>(), It.IsAny<bool>())).Returns(validationResults);
            _function = new PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger(
                _resourceHelper.Object,
                _httpResponseMessageHelper,
                _httpRequestHelper.Object,
                val.Object,
                _patchCustomerHttpTriggerService.Object,
                _jsonHelper,
                _loggerHelper.Object);
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x => x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customerString));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerRequestIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x=>x.GetResourceFromRequest<CustomerPatch>(_request)).Throws(new JsonException());

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _resourceHelper.Setup(x=>x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(false));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExistWhenCalledByService()
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x=>x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult<string>(null));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenUnableToUpdateCustomerRecord()
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x => x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customerString));
            _patchCustomerHttpTriggerService.Setup(x=>x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Models.Customer>(null));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsValid()
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x=>x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customerString));
            _patchCustomerHttpTriggerService.Setup(x=>x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(_customer));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        private async Task<HttpResponseMessage> RunFunction(string customerId)
        {
            return await _function.RunAsync(
                _request,
                _log.Object,
                customerId).ConfigureAwait(false);
        }
    }
}