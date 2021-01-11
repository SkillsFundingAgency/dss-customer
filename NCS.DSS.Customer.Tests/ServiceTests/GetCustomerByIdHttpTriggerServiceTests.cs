using Moq;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Tests.ServiceTests
{

    [TestFixture]
    public class GetCustomerByIdHttpTriggerServiceTests
    {

        private IGetCustomerByIdHttpTriggerService _customerByIdHttpTriggerService;
        private Mock<IDocumentDBProvider> _documentDbProvider;
        private Models.Customer _customer;
        private readonly Guid _customerId = Guid.Parse("58b43e3f-4a50-4900-9c82-a14682ee90fa");

        [SetUp]
        public void Setup()
        {
            _documentDbProvider = new Mock<IDocumentDBProvider>();
            _customerByIdHttpTriggerService = new GetCustomerByIdHttpTriggerService(_documentDbProvider.Object);
            _customer = new Models.Customer();
        }

        [Test]
        public async Task GetCustomerByIdHttpTriggerServiceTests_GetCustomerAsync_ReturnsNullWhenResourceCannotBeFound()
        {
            // Arrange
            _documentDbProvider.Setup(x=>x.GetCustomerByIdAsync(_customerId)).Returns(Task.FromResult<Models.Customer>(null));

            // Act
            var result = await _customerByIdHttpTriggerService.GetCustomerAsync(_customerId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCustomerByIdHttpTriggerServiceTests_GetCustomerAsync_ReturnsResource()
        {
            // Arrange
            _documentDbProvider.Setup(x=>x.GetCustomerByIdAsync(_customerId)).Returns(Task.FromResult(_customer));

            // Act
            var result = await _customerByIdHttpTriggerService.GetCustomerAsync(_customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Models.Customer>(result);
        }
    }
}
