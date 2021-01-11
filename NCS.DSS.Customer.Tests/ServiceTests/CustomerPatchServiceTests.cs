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
            Assert.IsNull(result);
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
            Assert.AreEqual(DateTime.MaxValue, customer.DateOfRegistration);
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
            Assert.AreEqual(Title.Dr, customer.Title);
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
            Assert.AreEqual(givenName, customer.GivenName);
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
            Assert.AreEqual(familyName, customer.FamilyName);
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
            Assert.AreEqual(DateTime.MaxValue, customer.DateofBirth);
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
            Assert.AreEqual(Gender.NotApplicable, customer.Gender);
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
            Assert.AreEqual("0000000111", customer.UniqueLearnerNumber);
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
            Assert.AreEqual(true, customer.OptInUserResearch);
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
            Assert.AreEqual(true, customer.OptInMarketResearch);
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
            Assert.AreEqual(DateTime.MaxValue, customer.DateOfTermination);
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
            Assert.AreEqual(ReasonForTermination.Duplicate, customer.ReasonForTermination);
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
            Assert.AreEqual(IntroducedBy.NotProvided, customer.IntroducedBy);
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
            Assert.AreEqual("More Info", customer.IntroducedByAdditionalInfo);
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
            Assert.AreEqual(DateTime.MaxValue, customer.LastModifiedDate);
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
            Assert.AreEqual("0000000111", customer.LastModifiedTouchpointId);
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
            Assert.AreEqual("0000000111", customer.SubcontractorId);
        }
    }
}
