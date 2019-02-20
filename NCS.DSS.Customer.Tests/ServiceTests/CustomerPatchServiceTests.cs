using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCS.DSS.Customer.Helpers;
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
        private JsonHelper _jsonHelper;
        private ICustomerPatchService _customerPatchService;
        private CustomerPatch _customerPatch;
        private string _json;

        [SetUp]
        public void Setup()
        {
            _jsonHelper = Substitute.For<JsonHelper>();
            _customerPatchService = Substitute.For<CustomerPatchService>();
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

            // Assert
            Assert.AreEqual(DateTime.MaxValue, patchedCustomer.DateOfRegistration);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckTitleIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { Title = Title.Dr };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(Title.Dr, patchedCustomer.Title);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckGivenNameIsUpdated_WhenPatchIsCalled()
        {
            var givenName = "John";
            var customerPatch = new CustomerPatch { GivenName = givenName };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(givenName, patchedCustomer.GivenName);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckFamilyNameIsUpdated_WhenPatchIsCalled()
        {
            var familyName = "Smith";
            var customerPatch = new CustomerPatch { FamilyName = familyName };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(familyName, patchedCustomer.FamilyName);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckDateofBirthIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { DateofBirth = DateTime.MaxValue };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, patchedCustomer.DateofBirth);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckGenderIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { Gender = Gender.NotApplicable };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(Gender.NotApplicable, patchedCustomer.Gender);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckUniqueLearnerNumberIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { UniqueLearnerNumber = "0000000111" };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual("0000000111", patchedCustomer.UniqueLearnerNumber);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckOptInUserResearchIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { OptInUserResearch = true };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(true, patchedCustomer.OptInUserResearch);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckOptInMarketResearchIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { OptInMarketResearch = true };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(true, patchedCustomer.OptInMarketResearch);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckDateOfTerminationIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { DateOfTermination = DateTime.MaxValue };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, patchedCustomer.DateOfTermination);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckReasonForTerminationIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { ReasonForTermination = ReasonForTermination.Duplicate };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(ReasonForTermination.Duplicate, patchedCustomer.ReasonForTermination);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckIntroducedByIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { IntroducedBy = IntroducedBy.NotProvided };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(IntroducedBy.NotProvided, patchedCustomer.IntroducedBy);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckIntroducedByAdditionalInfoIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { IntroducedByAdditionalInfo = "More Info" };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual("More Info", patchedCustomer.IntroducedByAdditionalInfo);
        }

        [Test]
        public void CustomerPatchServiceTests_CheckLastModifiedDatesUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { LastModifiedDate = DateTime.MaxValue };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, patchedCustomer.LastModifiedDate);
        }


        [Test]
        public void CustomerPatchServiceTests_CheckLastModifiedTouchpointIdIsUpdated_WhenPatchIsCalled()
        {
            var customerPatch = new CustomerPatch { LastModifiedTouchpointId = "0000000111" };

            var patchedCustomer = _customerPatchService.Patch(_json, customerPatch);

            // Assert
            Assert.AreEqual("0000000111", patchedCustomer.LastModifiedTouchpointId);
        }

    }
}