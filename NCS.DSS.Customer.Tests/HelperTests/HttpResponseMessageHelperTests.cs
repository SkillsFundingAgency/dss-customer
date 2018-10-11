using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using NCS.DSS.Customer.Helpers;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.HelperTests
{
    [TestFixture]
    public class HttpResponseMessageHelperTests
    {
        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeOK_WhenHttpResponseMessageOkIsCalledWithGuid()
        {
            var response = HttpResponseMessageHelper.Ok(Arg.Any<Guid>());

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeOK_WhenHttpResponseMessageOkIsCalledWithString()
        {
            var response = HttpResponseMessageHelper.Ok("");

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeCreated_WhenHttpResponseMessageCreatedIsCalledWithString()
        {
            var response = HttpResponseMessageHelper.Created("");

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeNoContent_WhenHttpResponseMessageNoContentIsCalled()
        {
            var response = HttpResponseMessageHelper.BadRequest();

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeBadRequest_WhenHttpResponseMessageBadRequestIsCalledWithGuid()
        {
            var response = HttpResponseMessageHelper.BadRequest(Arg.Any<Guid>());

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeUnprocessableEntity_WhenHttpResponseMessageUnprocessableEntityIsCalledWithRequest()
        {
            var response = HttpResponseMessageHelper.UnprocessableEntity(Arg.Any<HttpRequestMessage>());

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(422, (int)response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeUnprocessableEntity_WhenHttpResponseMessageUnprocessableEntityIsCalledWithValidationResult()
        {
            var response = HttpResponseMessageHelper.UnprocessableEntity(new List<ValidationResult>());

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(422, (int)response.StatusCode);
        }

        [Test]
        public void HttpResponseMessageHelperTests_ReturnsStatusCodeUnprocessableEntity_WhenHttpResponseMessageUnprocessableEntityIsCalledWithJsonException()
        {
            var response = HttpResponseMessageHelper.UnprocessableEntity(Arg.Any<JsonException>());

            Assert.IsInstanceOf<HttpResponseMessage>(response);
            Assert.AreEqual(422, (int) response.StatusCode);
        }
    }
}
