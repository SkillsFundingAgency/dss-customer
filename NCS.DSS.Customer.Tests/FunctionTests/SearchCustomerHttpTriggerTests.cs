using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.Search;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.SearchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Tests.FunctionTests
{
    [TestFixture]
    public class SearchCustomerHttpTriggerTests
    {
        private Mock<ILogger> _log;
        private HttpRequest _request;
        private Mock<IResourceHelper> _resourceHelper;
        private IValidate _validate;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private Mock<ISearchCustomerHttpTriggerService> _searchCustomerHttpTriggerService;
        private Models.Customer _customer;
        private Mock<IHttpRequestHelper> _httpRequestHelper;
        private IJsonHelper _jsonHelper;
        private Mock<ISearchIndexClient> _searchIndexClient;
        private Mock<ILoggerHelper> _loggerHelper;
        private SearchCustomerHttpTrigger.Function.SearchCustomerHttpTrigger _function;

        [SetUp]
        public void Setup()
        {
            _customer = new Models.Customer();
            _request = new DefaultHttpRequest(new DefaultHttpContext());

            _log = new Mock<ILogger>();
            _resourceHelper = new Mock<IResourceHelper>();
            _validate = new Validate();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _searchCustomerHttpTriggerService = new Mock<ISearchCustomerHttpTriggerService>();
            _jsonHelper = new JsonHelper();
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _searchIndexClient = new Mock<ISearchIndexClient>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _function = new SearchCustomerHttpTrigger.Function.SearchCustomerHttpTrigger(
                _log.Object,
                _resourceHelper.Object, 
                _httpRequestHelper.Object, 
                _searchCustomerHttpTriggerService.Object, 
                _httpResponseMessageHelper, 
                _jsonHelper, 
                _loggerHelper.Object);
        }

        [Test]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // Arrange
            _httpRequestHelper.Setup(x=>x.GetDssTouchpointId(_request)).Returns((string)null);

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        [Ignore("No Environment Variables")]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenQueryStringIsLessThan3Chars()
        {
            // Arrange
            _httpRequestHelper.Setup(x=>x.GetQueryString(_request, "GivenName")).Returns("AB");
            _httpRequestHelper.Setup(x=>x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x=>x.GetQueryString(_request, It.IsAny<string>())).Returns("TestValues");

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        [Ignore("No Environment Variables")]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenFamilyNameIsLessThan3Chars()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetQueryString(_request, "GivenName")).Returns("AB");
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x=>x.GetQueryString(_request, "FamilyName")).Returns("CD");

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        [Ignore("No Environment Variables")]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x=>x.GetDssTouchpointId(_request)).Returns("0000001");
            _searchCustomerHttpTriggerService.Setup(x=>x.SearchCustomerAsync(It.IsAny<ISearchIndexClient>(), It.IsAny<string>(), null, null, null)).Returns(Task.FromResult<List<Models.Customer>>(null));

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        [Ignore("No Environment Variables")]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeOk_WhenCustomerExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetQueryString(_request, "GivenName")).Returns("AB");
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _searchCustomerHttpTriggerService.Setup(x=>x.SearchCustomerAsync(It.IsAny<ISearchIndexClient>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>())).Returns(Task.FromResult<List<Models.Customer>>(new List<Models.Customer> { _customer }));

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }


        private async Task<HttpResponseMessage> RunFunction()
        {
            return await _function.Run(
                _request).ConfigureAwait(false);
        }

    }
}
