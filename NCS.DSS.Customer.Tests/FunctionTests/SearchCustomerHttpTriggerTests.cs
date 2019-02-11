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
using NCS.DSS.Customer.SearchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.FunctionTests
{
    [TestFixture]
    public class SearchCustomerHttpTriggerTests
    {
        private ILogger _log;
        private HttpRequestMessage _request;
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private IHttpRequestMessageHelper _httpRequestMessageHelper;
        private ISearchCustomerHttpTriggerService _searchCustomerHttpTriggerService;
        private Models.Customer _customer;

        [SetUp]
        public void Setup()
        {
            _customer = Substitute.For<Models.Customer>();

            _request = new HttpRequestMessage()
            {
                Content = new StringContent(string.Empty),
                RequestUri =
                    new Uri($"http://localhost:7071/api/Customers/7E467BDB-213F-407A-B86A-1954053D3C24/")
            };

            _log = Substitute.For<ILogger>();
            _resourceHelper = Substitute.For<IResourceHelper>();
            _validate = Substitute.For<IValidate>();
            _httpRequestMessageHelper = Substitute.For<IHttpRequestMessageHelper>();
            _searchCustomerHttpTriggerService = Substitute.For<ISearchCustomerHttpTriggerService>();
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns("0000000001");
            _httpRequestMessageHelper.GetQueryNameValuePairs(_request, Arg.Any<string>()).Returns("TestValues");
            _validate.ValidateResource(Arg.Any<CustomerPatch>(), Arg.Any<bool>()).Returns(new List<ValidationResult>());
        }

        [Test]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns((string)null);

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenQueryStringIsLessThan3Chars()
        {
            _httpRequestMessageHelper.GetQueryNameValuePairs(_request, "GivenName").Returns("AB");

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenFamilyNameIsLessThan3Chars()
        {
            _httpRequestMessageHelper.GetQueryNameValuePairs(_request, "FamilyName").Returns("CD");

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            _searchCustomerHttpTriggerService.SearchCustomerAsync(Arg.Any<string>()).Returns(Task.FromResult<List<Models.Customer>>(null).Result);

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task SearchCustomerHttpTrigger_ReturnsStatusCodeOk_WhenCustomerExist()
        {
            _searchCustomerHttpTriggerService.SearchCustomerAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<List<Models.Customer>>(new List<Models.Customer>{_customer}).Result);

            // Act
            var result = await RunFunction();

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }


        private async Task<HttpResponseMessage> RunFunction()
        {
            return await SearchCustomerHttpTrigger.Function.SearchCustomerHttpTrigger.Run(
                _request, _log, _resourceHelper, _httpRequestMessageHelper, _searchCustomerHttpTriggerService).ConfigureAwait(false);
        }

    }
}
