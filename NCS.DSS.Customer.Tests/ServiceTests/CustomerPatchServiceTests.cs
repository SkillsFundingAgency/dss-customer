using System;
using DFC.JSON.Standard;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.ReferenceData;
using Newtonsoft.Json;
using NSubstitute;
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
            _jsonHelper = Substitute.For<JsonHelper>();
            _customerPatchService = Substitute.For<CustomerPatchService>(_jsonHelper);
            _customerPatch = Substitute.For<CustomerPatch>();

            _json = JsonConvert.SerializeObject(_customerPatch);
        }

        [Test]
        public void CustomerPatchServiceTests_ReturnsNull_WhenOutcomePatchIsNull()
        {
            var result = _customerPatchService.Patch(string.Empty, Arg.Any<CustomerPatch>());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckDateOfRegistrationIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { DateOfRegistration = DateTime.MaxValue };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.DateOfRegistration);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckTitleIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { Title = Title.Dr };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(Title.Dr, customer.Title);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckGivenNameIsUpdated_WhenPatchIsCalled()
        {
            var givenName = "John";
            var customerPatch = new CustomerPatch { GivenName = givenName };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(givenName, customer.GivenName);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckFamilyNameIsUpdated_WhenPatchIsCalled()
        {
            var familyName = "Smith";
            var customerPatch = new CustomerPatch { FamilyName = familyName };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(familyName, customer.FamilyName);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckDateofBirthIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { DateofBirth = DateTime.MaxValue };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.DateofBirth);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckGenderIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { Gender = Gender.NotApplicable };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(Gender.NotApplicable, customer.Gender);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckUniqueLearnerNumberIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { UniqueLearnerNumber = "0000000111" };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual("0000000111", customer.UniqueLearnerNumber);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckOptInUserResearchIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { OptInUserResearch = true };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(true, customer.OptInUserResearch);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckOptInMarketResearchIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { OptInMarketResearch = true };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(true, customer.OptInMarketResearch);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckDateOfTerminationIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { DateOfTermination = DateTime.MaxValue };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.DateOfTermination);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckReasonForTerminationIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { ReasonForTermination = ReasonForTermination.Duplicate };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(ReasonForTermination.Duplicate, customer.ReasonForTermination);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckIntroducedByIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { IntroducedBy = IntroducedBy.NotProvided };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(IntroducedBy.NotProvided, customer.IntroducedBy);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckIntroducedByAdditionalInfoIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { IntroducedByAdditionalInfo = "More Info" };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual("More Info", customer.IntroducedByAdditionalInfo);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckLastModifiedDatesUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { LastModifiedDate = DateTime.MaxValue };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.LastModifiedDate);
        }


        [Test]
        public void CustomerPatchServiceTests_CheckLastModifiedTouchpointIdIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { LastModifiedTouchpointId = "0000000111" };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual("0000000111", customer.LastModifiedTouchpointId);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckSubcontractorIdIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { SubcontractorId = "0000000111" };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            var customer = JsonConvert.DeserializeObject<Models.Customer>(patchedCustomer);

            // Assert
            Assert.AreEqual("0000000111", customer.SubcontractorId);
        }
    }
}
