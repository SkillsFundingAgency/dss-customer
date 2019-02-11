using System;
using NCS.DSS.Customer.Cosmos.Helper;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.ServiceTests
{
    [TestFixture]
    public class DocumentDBHelperTests
    {
        [Test]
        public void DocumentDBHelperTests_ReturnsURI_WhenCreateDocumentUriIsCalled()
        {
            var uri = DocumentDBHelper.CreateDocumentUri(Arg.Any<Guid>());

            Assert.IsInstanceOf<Uri>(uri);
            Assert.IsNotNull(uri);
        }
        
        [Test]
        public void DocumentDBHelperTests_ReturnsURI_WhenDocumentCollectionUriIsCalled()
        {
            var uri = DocumentDBHelper.CreateDocumentCollectionUri();

            Assert.IsInstanceOf<Uri>(uri);
            Assert.IsNotNull(uri);
        }

        [Test]
        public void DocumentDBHelperTests_ReturnsURI_WhenCreateSubscriptionDocumentCollectionUriIsCalled()
        {
            var uri = DocumentDBHelper.CreateSubscriptionDocumentCollectionUri();

            Assert.IsInstanceOf<Uri>(uri);
            Assert.IsNotNull(uri);
        }
    }
}