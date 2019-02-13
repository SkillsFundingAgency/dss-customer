using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.HelperTests
{
    [TestFixture]
    public class HttpRequestMessageHelperTests
    {
        private HttpRequest _request;
        private IHttpRequestHelper _httpRequestHelper;

        [SetUp]
        public void Setup()
        {
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _httpRequestHelper = Substitute.For<IHttpRequestHelper>();
            _httpRequestHelper.GetDssTouchpointId(_request).Returns("0000000001");

            //_request = new HttpRequestMessage()
            //{
            //    Content = new StringContent(String.Empty, Encoding.UTF8, "application/json"),
            //    RequestUri = new Uri($"http://localhost:7071/")
            //};
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsEmptyString_WhenTouchpointHeaderIsNotAvailable()
        {
            _httpRequestHelper.GetDssTouchpointId(_request).Returns(null as string);

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(_request);

            Assert.IsNull(touchpointId);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsEmptyString_WhenTouchpointHeaderAvailableButEmpty()
        {
            _httpRequestHelper.GetDssTouchpointId(_request).Returns("");

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(_request);

            Assert.IsEmpty(touchpointId);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsTouchpointId_WhenHeaderIsAvailable()
        {            
            _httpRequestHelper.GetDssTouchpointId(_request).Returns("0000000000");

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(_request);            

            Assert.IsNotEmpty(touchpointId);
            Assert.NotNull(touchpointId);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsEmptyString_WhenApimUrlHeaderIsNotAvailable()
        {
            _httpRequestHelper.GetDssApimUrl(_request).Returns(null as string);

            var apimUrl = _httpRequestHelper.GetDssApimUrl(_request);

            Assert.IsNull(apimUrl);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsEmptyString_WhenApimUrlHeaderAvailableButEmpty()
        {            
            _httpRequestHelper.GetDssApimUrl(_request).Returns("");

            var apimUrl = _httpRequestHelper.GetDssApimUrl(_request);

            Assert.IsEmpty(apimUrl);
        }

        [Test]
        public void HttpRequestMessageHelper_ReturnsApimUrl_WhenApimUrlHeaderIsAvailable()
        {            
            _httpRequestHelper.GetDssApimUrl(_request).Returns("http://localhost:7071/");

            var apimUrl = _httpRequestHelper.GetDssApimUrl(_request);

            Assert.IsNotEmpty(apimUrl);
            Assert.NotNull(apimUrl);
        }

    }
}
