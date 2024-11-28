using Microsoft.Azure.Cosmos;
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
        private Mock<ICosmosDBProvider> _documentDbProvider;
        private Models.Customer _customer;
        private Mock<IServiceBusClient> _sbus;

        [SetUp]
        public void Setup()
        {
            _documentDbProvider = new Mock<ICosmosDBProvider>();
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
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PostCustomersHttpTriggerServiceTests_CreateAsync_ReturnsResourceWhenUpdated()
        {
            //Arrange
            var resourceResponse = new Mock<ItemResponse<Models.Customer>>();
            resourceResponse.Setup(x => x.Resource).Returns(_customer);
            resourceResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.Created);

            _documentDbProvider.Setup(x => x.CreateCustomerAsync(_customer)).Returns(Task.FromResult(resourceResponse.Object));

            // Act
            var result = await _customerHttpTriggerService.CreateNewCustomerAsync(_customer);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Models.Customer>());
        }
    }
}
