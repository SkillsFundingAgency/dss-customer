using DFC.HTTP.Standard;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.Common.Standard.Logging;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace NCS.DSS.Customer.Tests.FunctionTests
{
    [TestFixture]
    public class PatchCustomerHttpTriggerTests
    {
        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";
        private ILogger _log;
        private HttpRequest _request;
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private ILoggerHelper _loggerHelper;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IJsonHelper _jsonHelper;
        private IPatchCustomerHttpTriggerService _patchCustomerHttpTriggerService;
        private Models.Customer _customer;
        private CustomerPatch _customerPatch;

        [SetUp]
        public void Setup()
        {
            _customer = Substitute.For<Models.Customer>();
            _customerPatch = Substitute.For<CustomerPatch>();

            _request = new DefaultHttpRequest(new DefaultHttpContext());

            _log = Substitute.For<ILogger>();
            _resourceHelper = Substitute.For<IResourceHelper>();
            _validate = Substitute.For<IValidate>();
            _loggerHelper = Substitute.For<ILoggerHelper>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = Substitute.For<IHttpResponseMessageHelper>();
            _jsonHelper = Substitute.For<IJsonHelper>();
            _log = Substitute.For<ILogger>();
            _resourceHelper = Substitute.For<IResourceHelper>();
            _patchCustomerHttpTriggerService = Substitute.For<IPatchCustomerHttpTriggerService>();

            _httpRequestHelper.GetDssTouchpointId(_request).Returns("0000000001");
            _httpRequestHelper.GetDssApimUrl(_request).Returns("http://localhost:7071/");
            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            _httpRequestHelper.GetDssTouchpointId(_request).Returns((string)null);

            _httpResponseMessageHelper
                .BadRequest().Returns(x => new HttpResponseMessage(HttpStatusCode.BadRequest));

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            _httpResponseMessageHelper
                .BadRequest(Arg.Any<Guid>()).Returns(x => new HttpResponseMessage(HttpStatusCode.BadRequest));

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerHasFailedValidation()
        {
            var validationResults = new List<ValidationResult> { new ValidationResult("Customer Id is Required") };
            _validate.ValidateResource(Arg.Any<CustomerPatch>(), Arg.Any<bool>()).Returns(validationResults);

            _httpResponseMessageHelper
                .UnprocessableEntity(Arg.Any<List<ValidationResult>>())
                .Returns(x => new HttpResponseMessage((HttpStatusCode)422));

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerRequestIsInvalid()
        {
            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Throws(new JsonException());

            _httpResponseMessageHelper
                .UnprocessableEntity(Arg.Any<JsonException>()).Returns(x => new HttpResponseMessage((HttpStatusCode)422));

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(false);

            _httpResponseMessageHelper
                .NoContent(Arg.Any<Guid>()).Returns(x => new HttpResponseMessage(HttpStatusCode.NoContent));

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExistWhenCalledByService()
        {
            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<string>(null).Result);

            _httpResponseMessageHelper
                .NoContent(Arg.Any<Guid>()).Returns(x => new HttpResponseMessage(HttpStatusCode.NoContent));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenUnableToUpdateCustomerRecord()
        {
            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult("customer").Result);

            _patchCustomerHttpTriggerService.UpdateCosmosAsync(Arg.Any<Models.Customer>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            _httpResponseMessageHelper
                .BadRequest(Arg.Any<Guid>()).Returns(x => new HttpResponseMessage(HttpStatusCode.BadRequest));

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsNotValid()
        {
            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult("customer").Result);

            _patchCustomerHttpTriggerService.UpdateCosmosAsync(Arg.Any<Models.Customer>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            _httpResponseMessageHelper
                .BadRequest(Arg.Any<Guid>()).Returns(x => new HttpResponseMessage(HttpStatusCode.BadRequest));

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsValid()
        {
            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult("customer").Result);

            _patchCustomerHttpTriggerService.UpdateCosmosAsync(Arg.Any<Models.Customer>()).Returns(Task.FromResult(_customer).Result);

            _httpResponseMessageHelper
                .Ok(Arg.Any<string>()).Returns(x => new HttpResponseMessage(HttpStatusCode.OK));

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        private async Task<HttpResponseMessage> RunFunction(string customerId)
        {
            return await PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger.RunAsync(
                _request,
                _log,
                customerId,
                _resourceHelper,
                _httpResponseMessageHelper,
                _httpRequestHelper,
                _validate,
                _patchCustomerHttpTriggerService,
                _jsonHelper,
                _loggerHelper).ConfigureAwait(false);
        }

    }
}