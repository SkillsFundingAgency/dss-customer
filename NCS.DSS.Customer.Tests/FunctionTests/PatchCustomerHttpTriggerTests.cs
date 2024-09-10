using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
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
        private Mock<ILogger<PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger>> _logger;
        private Mock<IHttpRequestHelper> _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IJsonHelper _jsonHelper;
        private Mock<IPatchCustomerHttpTriggerService> _patchCustomerHttpTriggerService;
        private Models.Customer _customer;
        private CustomerPatch _customerPatch;
        private string _customerString;
        private Mock<IDocumentDBProvider> _provider;
        private PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger _function;
        private Mock<IDynamicHelper> _dynamicHelper;

        [SetUp]
        public void Setup()
        {
            _customer = new Models.Customer { IntroducedBy = ReferenceData.IntroducedBy.NotProvided };
            _customerPatch = new CustomerPatch();
            _request = new DefaultHttpContext().Request;

            _log = new Mock<ILogger>();
            _resourceHelper = new Mock<IResourceHelper>();
            _validate = new Validate();
            _logger = new Mock<ILogger<PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger>>();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _jsonHelper = new JsonHelper();
            _patchCustomerHttpTriggerService = new Mock<IPatchCustomerHttpTriggerService>();
            _customerString = JsonConvert.SerializeObject(_customer);
            _provider = new Mock<IDocumentDBProvider>();
            _dynamicHelper = new Mock<IDynamicHelper>();

            _function = new PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger(
                _resourceHelper.Object,
                _httpRequestHelper.Object,
                _validate,
                _patchCustomerHttpTriggerService.Object,
                _jsonHelper,
                _logger.Object,
                _provider.Object,
                _dynamicHelper.Object);
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns((string)null);

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerHasFailedValidation()
        {
            // Arrange
            var validationResults = new List<ValidationResult> { new ValidationResult("Customer Id is Required") };
            var val = new Mock<IValidate>();
            val.Setup(x => x.ValidateResource(It.IsAny<CustomerPatch>(), It.IsAny<bool>())).Returns(validationResults);
            _function = new PatchCustomerHttpTrigger.Function.PatchCustomerHttpTrigger(
                _resourceHelper.Object,
                _httpRequestHelper.Object,
                val.Object,
                _patchCustomerHttpTriggerService.Object,
                _jsonHelper,
                _logger.Object,
                _provider.Object,
                _dynamicHelper.Object);
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x => x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customerString));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenCustomerRequestIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Throws(new JsonException());

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(false));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExistWhenCalledByService()
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x => x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult<string>(null));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
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
            _patchCustomerHttpTriggerService.Setup(x => x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Models.Customer>(null));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsValid()
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x => x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customerString));
            _patchCustomerHttpTriggerService.Setup(x => x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(_customer));

            // Act
            var result = await RunFunction(ValidCustomerId);
            var responseResult = result as JsonResult;
            //Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(responseResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [TestCase("<script>alert(1)</script>")]
        [TestCase("Isobel.testing@dwp.gov.uk <script> Bridgewater JC")]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenIntroducedByAdditionalInfoRequestIsInValid(string additionalInfo)
        {
            // Arrange
            _customerPatch = new CustomerPatch { IntroducedByAdditionalInfo = additionalInfo };

            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x => x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customerString));
            _patchCustomerHttpTriggerService.Setup(x => x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(_customer));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());

            var unprocessableResult = result as UnprocessableEntityObjectResult;
            var errorList = unprocessableResult.Value as List<ValidationResult>;
            var error = errorList.FirstOrDefault().ErrorMessage;

            Assert.That(error.Contains("The field IntroducedByAdditionalInfo must match the regular expression"), Is.True);
        }

        [TestCase("Universal Credit work coach holly")]
        [TestCase("Isobel.testing@dwp.gov.uk Bridgewater PC")]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenIntroducedByAdditionalInfoRequestIsValid(string additionalInfo)
        {
            // Arrange
            _customerPatch = new CustomerPatch { IntroducedByAdditionalInfo = additionalInfo };

            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x => x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customerString));
            _patchCustomerHttpTriggerService.Setup(x => x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(_customer));

            // Act
            var result = await RunFunction(ValidCustomerId);
            var responseResult = result as JsonResult;
            //Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(responseResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [TestCase("<script>alert(1)</script>")]
        [TestCase("13231FDGD6")]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenSubcontractorIdRequestIsInValid(string subcontractorId)
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns(subcontractorId);
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x => x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customerString));
            _patchCustomerHttpTriggerService.Setup(x => x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(_customer));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());

            var unprocessableResult = result as UnprocessableEntityObjectResult;
            var errorList = unprocessableResult.Value as List<ValidationResult>;
            var error = errorList.FirstOrDefault().ErrorMessage;

            Assert.That(error.Contains("The field SubcontractorId must match the regular expression"), Is.True);
        }

        [TestCase("12345678910")]
        [TestCase("10001647")]
        public async Task PatchCustomerHttpTrigger_ReturnsStatusCodeOK_WhenSubcontractorIdRequestIsValid(string subcontractorId)
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns(subcontractorId);
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<CustomerPatch>(_request)).Returns(Task.FromResult(_customerPatch));
            _patchCustomerHttpTriggerService.Setup(x => x.GetCustomerByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customerString));
            _patchCustomerHttpTriggerService.Setup(x => x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(_customer));

            // Act
            var result = await RunFunction(ValidCustomerId);
            var responseResult = result as JsonResult;
            //Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(responseResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        private async Task<IActionResult> RunFunction(string customerId)
        {
            return await _function.RunAsync(
                _request,
                customerId).ConfigureAwait(false);
        }
    }
}