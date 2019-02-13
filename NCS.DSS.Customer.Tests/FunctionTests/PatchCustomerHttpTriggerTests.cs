using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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

namespace NCS.DSS.Customer.Tests.FunctionTests
{
    [TestFixture]
    public class PatchCustomerHttpTriggerTests
    {
        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string ValidInteractionId = "1e1a555c-9633-4e12-ab28-09ed60d51cb3";
        private const string ValidActionPlanId = "cff8080e-1da2-42bd-9b63-8f235aad9d86";
        private const string ValidOutcomeId = "d5369b9a-6959-4bd3-92fc-1583e72b7e51";
        private const string ValidSessionId = "cff8080e-1da2-42bd-9b63-8f235aad9d86";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";
        private ILogger _log;
        private HttpRequest _request;
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private IValidate _substitutedValidate;
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
            _validate = new Validate();
            _substitutedValidate = Substitute.For<IValidate>();
            _loggerHelper = Substitute.For<ILoggerHelper>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _jsonHelper = Substitute.For<IJsonHelper>();
            _log = Substitute.For<ILogger>();
            _resourceHelper = Substitute.For<IResourceHelper>();
            _patchCustomerHttpTriggerService = Substitute.For<IPatchCustomerHttpTriggerService>();
            _httpRequestHelper.GetDssTouchpointId(_request).Returns("0000000001");
            _httpRequestHelper.GetDssApimUrl(_request).Returns("http://localhost:7071/");
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            _httpRequestHelper.GetDssTouchpointId(_request).Returns((string)null);

            // Act
            var result = await RunFunction(InValidId, _validate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Act
            var result = await RunFunction(InValidId, _validate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerHasFailedValidation()
        {
            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            var validationResults = new List<ValidationResult> { new ValidationResult("interaction Id is Required") };
            _substitutedValidate.ValidateResource(Arg.Any<CustomerPatch>(), Arg.Any<bool>()).Returns(validationResults);

            var result = await RunFunction(ValidCustomerId, _substitutedValidate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerRequestIsInvalid()
        {
            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Throws(new JsonException());

            var result = await RunFunction(ValidCustomerId, _validate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(false);

            var result = await RunFunction(ValidCustomerId, _validate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExistWhenCalledByService()
        {
            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            // Act
            var result = await RunFunction(ValidCustomerId, _validate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOk_WhenCustomerDoesNotExist()
        {
            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            // Act
            var result = await RunFunction(ValidCustomerId, _validate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenUnableToUpdateCustomerRecord()
        {
            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(_customer).Result);

            _patchCustomerHttpTriggerService.UpdateCosmosAsync(Arg.Any<Models.Customer>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            var result = await RunFunction(ValidCustomerId, _validate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsNotValid()
        {
            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(_customer).Result);

            _patchCustomerHttpTriggerService.UpdateCosmosAsync(Arg.Any<Models.Customer>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            var result = await RunFunction(ValidCustomerId, _validate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsValid()
        {
            _httpRequestHelper.GetResourceFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(_customer).Result);

            _patchCustomerHttpTriggerService.UpdateCosmosAsync(Arg.Any<Models.Customer>()).Returns(Task.FromResult(_customer).Result);

            var result = await RunFunction(ValidCustomerId, _validate);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        private async Task<HttpResponseMessage> RunFunction(string customerId, IValidate validator)
        {
            return await PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger.RunAsync(
                _request, _log, customerId, _resourceHelper, _httpResponseMessageHelper, _httpRequestHelper, validator, _patchCustomerHttpTriggerService, _jsonHelper, _loggerHelper).ConfigureAwait(false);
        }

    }
}