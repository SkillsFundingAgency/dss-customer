using System;
using System.Threading.Tasks;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.ServiceTests
{

    [TestFixture]
    public class GetCustomerByIdHttpTriggerServiceTests
    {

        private IGetCustomerByIdHttpTriggerService _customerByIdHttpTriggerService;
        private IDocumentDBProvider _documentDbProvider;
        private Models.Customer _customer;
        private readonly Guid _customerId = Guid.Parse("58b43e3f-4a50-4900-9c82-a14682ee90fa");

        [SetUp]
        public void Setup()
        {
            _documentDbProvider = Substitute.For<IDocumentDBProvider>();
            _customerByIdHttpTriggerService = Substitute.For<GetCustomerByIdHttpTriggerService>(_documentDbProvider);
            _customer = Substitute.For<Models.Customer>();
        }

        [Test]
        public async Task GetCustomerByIdHttpTriggerServiceTests_GetCustomerAsync_ReturnsNullWhenResourceCannotBeFound()
        {
            _documentDbProvider.GetCustomerByIdAsync(_customerId).Returns(Task.FromResult<Models.Customer>(null).Result);

            // Act
            var result = await _customerByIdHttpTriggerService.GetCustomerAsync(_customerId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCustomerByIdHttpTriggerServiceTests_GetCustomerAsync_ReturnsResource()
        {
            _documentDbProvider.GetCustomerByIdAsync(_customerId).Returns(Task.FromResult(_customer).Result);

            // Act
            var result = await _customerByIdHttpTriggerService.GetCustomerAsync(_customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Models.Customer>(result);
        }
    }
}
