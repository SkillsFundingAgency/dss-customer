using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.ServiceBus;
using NUnit.Framework;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Tests.ServiceTests
{

    [TestFixture]
    public class PostCustomerHttpTriggerTests
    {
        private IPostCustomerHttpTriggerService _customerHttpTriggerService;
        private Mock<IDocumentDBProvider> _documentDbProvider;
        private Models.Customer _customer;
        private Mock<IServiceBusClient> _sbus;

        [SetUp]
        public void Setup()
        {
            _documentDbProvider = new Mock<IDocumentDBProvider>();
            _sbus = new Mock<IServiceBusClient>();
            _customerHttpTriggerService = new PostCustomerHttpTriggerService(_documentDbProvider.Object, _sbus.Object);
            _customer = new Models.Customer();
        }

        [Test]
        public async Task PostCustomersHttpTriggerServiceTests_CreateAsync_ReturnsNullWhenCustomerJsonIsNullOrEmpty()
        {
            // Act
            var result = await _customerHttpTriggerService.CreateNewCustomerAsync(null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task PostCustomersHttpTriggerServiceTests_CreateAsync_ReturnsResourceWhenUpdated()
        {
            // Arrange
            const string documentServiceResponseClass = "Microsoft.Azure.Documents.DocumentServiceResponse, Microsoft.Azure.DocumentDB.Core, Version=2.2.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
            const string dictionaryNameValueCollectionClass = "Microsoft.Azure.Documents.Collections.DictionaryNameValueCollection, Microsoft.Azure.DocumentDB.Core, Version=2.2.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

            var resourceResponse = new ResourceResponse<Document>(new Document());
            var documentServiceResponseType = Type.GetType(documentServiceResponseClass);

            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var headers = new NameValueCollection { { "x-ms-request-charge", "0" } };

            var headersDictionaryType = Type.GetType(dictionaryNameValueCollectionClass);

            var headersDictionaryInstance = Activator.CreateInstance(headersDictionaryType, headers);

            var arguments = new[] { Stream.Null, headersDictionaryInstance, HttpStatusCode.Created, null };

            var documentServiceResponse = documentServiceResponseType.GetTypeInfo().GetConstructors(flags)[0].Invoke(arguments);

            var responseField = typeof(ResourceResponse<Document>).GetTypeInfo().GetField("response", flags);

            responseField?.SetValue(resourceResponse, documentServiceResponse);

            _documentDbProvider.Setup(x=>x.CreateCustomerAsync(_customer)).Returns(Task.FromResult(resourceResponse));

            // Act
            var result = await _customerHttpTriggerService.CreateNewCustomerAsync(_customer);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Models.Customer>(result);
        }
    }
}
