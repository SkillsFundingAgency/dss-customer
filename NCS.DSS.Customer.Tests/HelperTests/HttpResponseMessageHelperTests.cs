using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;

namespace NCS.DSS.Customer.Tests.HelperTests
{
    [TestFixture]
    public class HttpResponseMessageHelperTests
    {
        private IHttpResponseMessageHelper _httpResponseHelper;
        [SetUp]
        public void Setup()
        {
            _httpResponseHelper = new HttpResponseMessageHelper();
        }
        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeOK_WhenHttpResponseMessageOkIsCalledWithGuid()
        {
            var response = _httpResponseHelper.Ok(Arg.Any<Guid>()); 

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeOK_WhenHttpResponseMessageOkIsCalledWithString()
        {            
            var response = _httpResponseHelper.Ok("");

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeCreated_WhenHttpResponseMessageCreatedIsCalledWithString()
        {
            var response = _httpResponseHelper.Created("");

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeNoContent_WhenHttpResponseMessageNoContentIsCalled()
        {
            var response = _httpResponseHelper.BadRequest();

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeBadRequest_WhenHttpResponseMessageBadRequestIsCalledWithGuid()
        {
            var response = _httpResponseHelper.BadRequest(Arg.Any<Guid>());

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeUnprocessableEntity_WhenHttpResponseMessageUnprocessableEntityIsCalledWithRequest()
        {
            var response = _httpResponseHelper.UnprocessableEntity(Arg.Any<HttpRequest>());

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(422, (int)response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeUnprocessableEntity_WhenHttpResponseMessageUnprocessableEntityIsCalledWithValidationResult()
        {
            var response = _httpResponseHelper.UnprocessableEntity(new List<ValidationResult>());

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(422, (int)response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeUnprocessableEntity_WhenHttpResponseMessageUnprocessableEntityIsCalledWithJsonException()
        {
            var response = _httpResponseHelper.UnprocessableEntity(Arg.Any<JsonException>());

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(422, (int) response.StatusCode);
        }
    }
}
