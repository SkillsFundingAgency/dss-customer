using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Tests.FunctionTests
{
    [TestFixture]
    public class GetCustomerByIdHttpTriggerTests
    {
        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";

        private Mock<ILogger> _log;
        private HttpRequest _request;
        private Mock<IResourceHelper> _resourceHelper;
        private Mock<ILogger<GetCustomerByIdHttpTrigger.Function.GetCustomerByIdHttpTrigger>> _logger;
        private Mock<IHttpRequestHelper> _httpRequestHelper;
        private IJsonHelper _jsonHelper;
        private Mock<IGetCustomerByIdHttpTriggerService> _getCustomerByIdHttpTriggerService;
        private Models.Customer _customer;
        private Mock<IDocumentDBProvider> _documentDbProvider;
        private GetCustomerByIdHttpTrigger.Function.GetCustomerByIdHttpTrigger _function;

        [SetUp]
        public void Setup()
        {
            _customer = new Models.Customer();
            _request = new DefaultHttpContext().Request;
            _log = new Mock<ILogger>();
            _resourceHelper = new Mock<IResourceHelper>();
            _logger = new Mock<ILogger<GetCustomerByIdHttpTrigger.Function.GetCustomerByIdHttpTrigger>>();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _jsonHelper = new JsonHelper();
            _documentDbProvider = new Mock<IDocumentDBProvider>();
            _getCustomerByIdHttpTriggerService = new Mock<IGetCustomerByIdHttpTriggerService>();
            _function = new GetCustomerByIdHttpTrigger.Function.GetCustomerByIdHttpTrigger(
                _resourceHelper.Object,
                _getCustomerByIdHttpTriggerService.Object,
                _logger.Object,
                _httpRequestHelper.Object,
                _jsonHelper);
        }

        [Test]
        public async Task GetCustomerByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // Arrange
            _httpRequestHelper.Setup(x=>x.GetDssTouchpointId(_request)).Returns((string)null);

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetCustomerByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x=>x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x=>x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");

            // Act
            var result = await RunFunction(InValidId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetCustomerByIdHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _getCustomerByIdHttpTriggerService.Setup(x=>x.GetCustomerAsync(It.IsAny<Guid>())).Returns(Task.FromResult<Models.Customer>(null));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task GetCustomerByIdHttpTrigger_ReturnsStatusCodeOk_WhenCustomerExists()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:7071/");
            _getCustomerByIdHttpTriggerService.Setup(x => x.GetCustomerAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_customer));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        private async Task<IActionResult> RunFunction(string customerId)
        {
            return await _function.Run(
                _request, customerId).ConfigureAwait(false);
        }

    }
}