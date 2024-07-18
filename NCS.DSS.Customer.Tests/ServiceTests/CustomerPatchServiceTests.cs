using System;
using DFC.JSON.Standard;
using Moq;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.ReferenceData;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.ServiceTests
{

    [TestFixture]
    public class CustomerPatchServiceTests
    {
        private IJsonHelper _jsonHelper;
        private ICustomerPatchService _customerPatchService;
        private CustomerPatch _customerPatch;
        private string _json;

        [SetUp]
        public void Setup()
        {
            _jsonHelper = new JsonHelper();
            _customerPatchService = new CustomerPatchService(_jsonHelper);
            _customerPatch = new CustomerPatch();
            _json = JsonConvert.SerializeObject(_customerPatch);
        }

        [Test]
        public void CustomerPatchServiceTests_ReturnsNull_WhenOutcomePatchIsNull()
        {
            // Act
            var result = _customerPatchService.Patch(string.Empty, It.IsAny<CustomerPatch>());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckDateOfRegistrationIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { DateOfRegistration = DateTime.MaxValue };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.DateOfRegistration, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckTitleIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { Title = Title.Dr };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.Title, Is.EqualTo(Title.Dr));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckGivenNameIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var givenName = "John";
            var customerPatch = new CustomerPatch { GivenName = givenName };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.GivenName, Is.EqualTo(givenName));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckFamilyNameIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var familyName = "Smith";
            var customerPatch = new CustomerPatch { FamilyName = familyName };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.FamilyName, Is.EqualTo(familyName));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckDateofBirthIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { DateofBirth = DateTime.MaxValue };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.DateofBirth, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckGenderIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { Gender = Gender.NotApplicable };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.Gender, Is.EqualTo(Gender.NotApplicable));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckUniqueLearnerNumberIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { UniqueLearnerNumber = "0000000111" };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.UniqueLearnerNumber, Is.EqualTo("0000000111"));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckOptInUserResearchIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { OptInUserResearch = true };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.OptInUserResearch, Is.True);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckOptInMarketResearchIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { OptInMarketResearch = true };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.OptInMarketResearch, Is.True);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckDateOfTerminationIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { DateOfTermination = DateTime.MaxValue };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.DateOfTermination, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckReasonForTerminationIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { ReasonForTermination = ReasonForTermination.Duplicate };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.ReasonForTermination, Is.EqualTo(ReasonForTermination.Duplicate));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckIntroducedByIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { IntroducedBy = IntroducedBy.NotProvided };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.IntroducedBy, Is.EqualTo(IntroducedBy.NotProvided));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckIntroducedByAdditionalInfoIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { IntroducedByAdditionalInfo = "More Info" };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.IntroducedByAdditionalInfo, Is.EqualTo("More Info"));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckLastModifiedDatesUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { LastModifiedDate = DateTime.MaxValue };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.LastModifiedDate, Is.EqualTo(DateTime.MaxValue));
        }


        [Test]
        public void CustomerPatchServiceTests_CheckLastModifiedTouchpointIdIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { LastModifiedTouchpointId = "0000000111" };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.LastModifiedTouchpointId, Is.EqualTo("0000000111"));
        }

        [Test]
        public void CustomerPatchServiceTests_CheckSubcontractorIdIsUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var customerPatch = new CustomerPatch { SubcontractorId = "0000000111" };

            // Act
            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);
            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.That(customer.SubcontractorId, Is.EqualTo("0000000111"));
        }
    }
}
