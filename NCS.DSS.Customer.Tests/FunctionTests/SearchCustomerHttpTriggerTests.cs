using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.Search;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.SearchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Tests.FunctionTests
{
    [TestFixture]
    public class SearchCustomerHttpTriggerTests
    {
        private ILogger _log;
        private HttpRequest _request;
        private IResourceHelper _resourceHelper;        
        private IValidate _validate;        
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private ISearchCustomerHttpTriggerService _searchCustomerHttpTriggerService;
        private Models.Customer _customer;
        private IHttpRequestHelper _httpRequestHelper;
        private IJsonHelper _jsonHelper;
        private ISearchIndexClient _searchIndexClient;
        private ILoggerHelper _loggerHelper;

        [SetUp]
        public void Setup()
        {
            _customer = Substitute.For<Models.Customer>();

            _request = new DefaultHttpRequest(new DefaultHttpContext());

            //_request = new HttpRequestMessage()
            //{
            //    Content = new StringContent(string.Empty),
            //    RequestUri =
            //        new Uri($"http://localhost:7071/api/Customers/7E467BDB-213F-407A-B86A-1954053D3C24/")
            //};

            _log = Substitute.For<ILogger>();
            _resourceHelper = Substitute.For<IResourceHelper>();
            _validate = Substitute.For<IValidate>();
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _searchCustomerHttpTriggerService = Substitute.For<ISearchCustomerHttpTriggerService>();
            _httpRequestHelper.GetDssTouchpointId(_request).Returns("0000000001");
            _httpRequestHelper.GetQueryString(_request, Arg.Any<string>()).Returns("TestValues");
            _validate.ValidateResource(Arg.Any<CustomerPatch>(), Arg.Any<bool>()).Returns(new List<ValidationResult>());
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _jsonHelper = Substitute.For<IJsonHelper>();
            _httpResponseMessageHelper = new HttpResponseMessageHelper(); //Substitute.For<IHttpResponseMessageHelper>();
            _searchIndexClient = Substitute.For<ISearchIndexClient>();
            _loggerHelper = Substitute.For<ILoggerHelper>();
        }

        [Test]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            _httpRequestHelper.GetDssTouchpointId(_request).Returns((string)null);

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
            _httpRequestHelper.GetQueryString(_request, "GivenName").Returns("AB");

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
            _httpRequestHelper.GetQueryString(_request, "FamilyName").Returns("CD");

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
            _httpRequestHelper.GetDssTouchpointId(_request).Returns("0000001");

            _searchCustomerHttpTriggerService.SearchCustomerAsync(Arg.Any<ISearchIndexClient>(), Arg.Any<string>()).Returns(Task.FromResult<List<Models.Customer>>(null).Result);

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
            _searchCustomerHttpTriggerService.SearchCustomerAsync(Arg.Any<ISearchIndexClient>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<string>>(), Arg.Any<List<string>>()).Returns(Task.FromResult<List<Models.Customer>>(new List<Models.Customer>{_customer}).Result);

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }


        private async Task<HttpResponseMessage> RunFunction()
        {
            return await SearchCustomerHttpTrigger.Function.SearchCustomerHttpTrigger.Run(
                _request, _log, _resourceHelper, _httpRequestHelper, _searchCustomerHttpTriggerService, _httpResponseMessageHelper,
                _jsonHelper, _loggerHelper).ConfigureAwait(false);
        }

    }
}
