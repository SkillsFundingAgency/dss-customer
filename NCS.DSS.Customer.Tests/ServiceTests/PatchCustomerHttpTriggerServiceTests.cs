using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.ServiceBus;
using Newtonsoft.Json;
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
    public class PatchCustomerHttpTriggerServiceTests
    {
        private IPatchCustomerHttpTriggerService _patchCustomerHttpTrigger;
        private Mock<ICustomerPatchService> _customerPatchService;
        private Mock<IDocumentDBProvider> _documentDbProvider;
        private Mock<IServiceBusClient> _sbus;
        private Models.Customer _customer;
        private CustomerPatch _customerPatch;
        private string _json;
        private string _customerString;
        private Guid _customerId = Guid.Parse("7E467BDB-213F-407A-B86A-1954053D3C24");


        [SetUp]
        public void Setup()
        {
            _documentDbProvider = new Mock<IDocumentDBProvider>();
            _customerPatchService = new Mock<ICustomerPatchService>();
            _sbus = new Mock<IServiceBusClient>();
            _patchCustomerHttpTrigger = new PatchCustomerHttpTriggerService(_customerPatchService.Object, _documentDbProvider.Object, _sbus.Object);
            _customer = new Models.Customer();
            _customerPatch = new CustomerPatch();
            _json = JsonConvert.SerializeObject(_customerPatch);
            _customerString = JsonConvert.SerializeObject(_customer);
        }

        [Test]
        public void PatchCustomerHttpTriggerServiceTests_PatchResource_ReturnsNullWhenCustomerJsonIsNullOrEmpty()
        {
            // Act
            var result = _patchCustomerHttpTrigger.PatchResource(null, It.IsAny<CustomerPatch>());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PatchCustomerHttpTriggerServiceTests_UpdateCosmosAsync_ReturnsNullWhenCustomerPatchServicePatchJsonIsNullOrEmpty()
        {
            // Arrange
            _customerPatchService.Setup(x => x.Patch(It.IsAny<string>(), It.IsAny<CustomerPatch>())).Returns<string>(null);

            // Act
            var result = await _patchCustomerHttpTrigger.UpdateCosmosAsync(_customerString, _customerId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PatchCustomerHttpTriggerServiceTests_UpdateCosmosAsync_ReturnsNullWhenResourceCannotBeUpdated()
        {
            // Arrange
            _documentDbProvider.Setup(x => x.UpdateCustomerAsync(_customerString, _customerId)).Returns(Task.FromResult<ItemResponse<Models.Customer>>(null));

            // Act
            var result = await _patchCustomerHttpTrigger.UpdateCosmosAsync(_customerString, _customerId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PatchCustomerHttpTriggerServiceTests_UpdateCosmosAsync_ReturnsResourceWhenUpdated()
        {
            //Arrange
            var resourceResponse = new Mock<ItemResponse<Models.Customer>>();
            resourceResponse.Setup(x => x.Resource).Returns(_customer);
            resourceResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            
            _documentDbProvider.Setup(x => x.UpdateCustomerAsync(_customerString, _customerId)).Returns(Task.FromResult(resourceResponse.Object));

            // Act
            var result = await _patchCustomerHttpTrigger.UpdateCosmosAsync(_customerString, _customerId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Models.Customer>());
        }

        [Test]
        public async Task PatchCustomerHttpTriggerServiceTests_GetCustomerByIdAsync_ReturnsNullWhenResourceHasNotBeenFound()
        {
            // Arrange
            _documentDbProvider.Setup(x => x.GetCustomerByIdForUpdateAsync(_customerId)).Returns(Task.FromResult<string>(string.Empty));

            // Act
            var result = await _patchCustomerHttpTrigger.GetCustomerByIdAsync(_customerId);

            // Assert
            Assert.That(result, Is.InstanceOf<string>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task PatchCustomerHttpTriggerServiceTests_GetCustomerByIdAsync_ReturnsResourceWhenResourceHasBeenFound()
        {
            // Arrange
            _documentDbProvider.Setup(x => x.GetCustomerByIdForUpdateAsync(_customerId)).Returns(Task.FromResult(_json));

            // Act
            var result = await _patchCustomerHttpTrigger.GetCustomerByIdAsync(_customerId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<string>());
        }
    }
}
