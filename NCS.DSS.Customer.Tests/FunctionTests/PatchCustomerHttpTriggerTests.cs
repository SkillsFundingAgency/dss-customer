using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.FunctionTests
{
    [TestFixture]
    public class PatchCustomerHttpTriggerTests
    {
        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";
        private ILogger _log;
        private HttpRequestMessage _request;
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private IHttpRequestMessageHelper _httpRequestMessageHelper;
        private IPatchCustomerHttpTriggerService _patchCustomerHttpTriggerService;
        private Models.Customer _customer;
        private CustomerPatch _customerPatch;

        [SetUp]
        public void Setup()
        {
            _customer = Substitute.For<Models.Customer>();
            _customerPatch = Substitute.For<CustomerPatch>();

            _request = new HttpRequestMessage()
            {
                Content = new StringContent(string.Empty),
                RequestUri =
                    new Uri($"http://localhost:7071/api/Customers/7E467BDB-213F-407A-B86A-1954053D3C24/" +
                            $"Customers/1e1a555c-9633-4e12-ab28-09ed60d51cb3")
            };

            _log = Substitute.For<ILogger>();
            _resourceHelper = Substitute.For<IResourceHelper>();
            _validate = Substitute.For<IValidate>();
            _httpRequestMessageHelper = Substitute.For<IHttpRequestMessageHelper>();
            _patchCustomerHttpTriggerService = Substitute.For<IPatchCustomerHttpTriggerService>();
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns("0000000001");
            _httpRequestMessageHelper.GetApimURL(_request).Returns("http://localhost:7071/");
            _validate.ValidateResource(Arg.Any<CustomerPatch>(), Arg.Any<bool>()).Returns(new List<ValidationResult>());
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns((string)null);

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerHasFailedValidation()
        {
            _httpRequestMessageHelper.GetCustomerFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            var validationResults = new List<ValidationResult> { new ValidationResult("interaction Id is Required") };
            _validate.ValidateResource(Arg.Any<CustomerPatch>(), Arg.Any<bool>()).Returns(validationResults);

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerRequestIsInvalid()
        {
            _httpRequestMessageHelper.GetCustomerFromRequest<CustomerPatch>(_request).Throws(new JsonException());

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            _httpRequestMessageHelper.GetCustomerFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(false);

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExistWhenCalledByService()
        {
            _httpRequestMessageHelper.GetCustomerFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOk_WhenCustomerDoesNotExist()
        {
            _httpRequestMessageHelper.GetCustomerFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenUnableToUpdateCustomerRecord()
        {
            _httpRequestMessageHelper.GetCustomerFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(_customer).Result);

            _patchCustomerHttpTriggerService.UpdateCustomerAsync(Arg.Any<Models.Customer>(), Arg.Any<CustomerPatch>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsNotValid()
        {
            _httpRequestMessageHelper.GetCustomerFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(_customer).Result);

            _patchCustomerHttpTriggerService.UpdateCustomerAsync(Arg.Any<Models.Customer>(), Arg.Any<CustomerPatch>()).Returns(Task.FromResult<Models.Customer>(null).Result);

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsValid()
        {
            _httpRequestMessageHelper.GetCustomerFromRequest<CustomerPatch>(_request).Returns(Task.FromResult(_customerPatch).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            _patchCustomerHttpTriggerService.GetCustomerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(_customer).Result);

            _patchCustomerHttpTriggerService.UpdateCustomerAsync(Arg.Any<Models.Customer>(), Arg.Any<CustomerPatch>()).Returns(Task.FromResult(_customer).Result);

            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        private async Task<HttpResponseMessage> RunFunction(string customerId)
        {
            return await PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger.RunAsync(
                _request, _log, customerId, _resourceHelper, _httpRequestMessageHelper, _validate, _patchCustomerHttpTriggerService).ConfigureAwait(false);
        }

    }
}