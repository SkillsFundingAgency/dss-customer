using System;
using System.Net.Http;
using System.Text;
using NCS.DSS.Customer.Helpers;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.HelperTests
{
    [TestFixture]
    public class HttpRequestMessageHelperTests
    {
        private HttpRequestMessage _request;

        [SetUp]
        public void Setup()
        {
            _request = new HttpRequestMessage()
            {
                Content = new StringContent(String.Empty, Encoding.UTF8, "application/json"),
                RequestUri = new Uri($"http://localhost:7071/")
            };
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsEmptyString_WhenTouchpointHeaderIsNotAvailable()
        {
            var httpRequestMessageHelper = new HttpRequestMessageHelper();

            var touchpointId = httpRequestMessageHelper.GetTouchpointId(_request);

            Assert.IsNull(touchpointId);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsEmptyString_WhenTouchpointHeaderAvailableButEmpty()
        {
            var httpRequestMessageHelper = new HttpRequestMessageHelper();
            _request.Headers.Add("TouchpointId", "");

            var touchpointId = httpRequestMessageHelper.GetTouchpointId(_request);

            Assert.IsEmpty(touchpointId);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsTouchpointId_WhenHeaderIsAvailable()
        {
            var httpRequestMessageHelper = new HttpRequestMessageHelper();

            _request.Headers.Add("TouchpointId", "0000000000");

            var touchpointId = httpRequestMessageHelper.GetTouchpointId(_request);

            Assert.IsNotEmpty(touchpointId);
            Assert.NotNull(touchpointId);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsEmptyString_WhenApimUrlHeaderIsNotAvailable()
        {
            var httpRequestMessageHelper = new HttpRequestMessageHelper();

            var apimUrl = httpRequestMessageHelper.GetApimURL(_request);

            Assert.IsNull(apimUrl);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsEmptyString_WhenApimUrlHeaderAvailableButEmpty()
        {
            var httpRequestMessageHelper = new HttpRequestMessageHelper();
            _request.Headers.Add("apimurl", "");

            var apimUrl = httpRequestMessageHelper.GetApimURL(_request);

            Assert.IsEmpty(apimUrl);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsApimUrl_WhenApimUrlHeaderIsAvailable()
        {
            var httpRequestMessageHelper = new HttpRequestMessageHelper();

            _request.Headers.Add("apimurl", "http://localhost:7071/");

            var apimUrl = httpRequestMessageHelper.GetApimURL(_request);

            Assert.IsNotEmpty(apimUrl);
            Assert.NotNull(apimUrl);
        }

    }
}
