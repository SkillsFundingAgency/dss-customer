using System;
using NCS.DSS.Customer.ReferenceData;
using NUnit.Framework;

namespace NCS.DSS.Customer.Tests.ModelTests
{
    [TestFixture]
    public class CustomerTests
    {
        [Test]
        public void CustomerTests_PopulatesDefaultValues_WhenSetDefaultValuesIsCalled()
        {
            var customer = new Models.Customer();
            customer.SetDefaultValues();

            // Assert
            Assert.IsNotNull(customer.DateOfRegistration);
            Assert.IsNotNull(customer.LastModifiedDate);
            Assert.AreEqual(false, customer.OptInUserResearch);
            Assert.AreEqual(false, customer.OptInMarketResearch);
            Assert.AreEqual(Title.NotProvided, customer.Title);
            Assert.AreEqual(Gender.NotProvided, customer.Gender);
            Assert.AreEqual(IntroducedBy.NotProvided, customer.IntroducedBy);
        }

        [Test]
        public void CustomerTests_CheckDateOfRegistrationDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var customer = new Models.Customer {DateOfRegistration = DateTime.MaxValue};
            
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.DateOfRegistration);
        }

        [Test]
        public void CustomerTests_CheckLastModifiedDateDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var customer = new Models.Customer {LastModifiedDate = DateTime.MaxValue};
            
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.LastModifiedDate);
        }

        [Test]
        public void CustomerTests_CheckOptInUserResearchDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var customer = new Models.Customer { OptInUserResearch = true };

            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(true, customer.OptInUserResearch);
        }

        [Test]
        public void CustomerTests_CheckOptInMarketResearchDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var customer = new Models.Customer { OptInMarketResearch = true };

            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(true, customer.OptInMarketResearch);
        }

        [Test]
        public void CustomerTests_CheckTitleDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var customer = new Models.Customer { Title = Title.Mr };

            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(Title.Mr, customer.Title);
        }

        [Test]
        public void CustomerTests_CheckGenderDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var customer = new Models.Customer { Gender = Gender.Male };

            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(Gender.Male, customer.Gender);
        }

        [Test]
        public void CustomerTests_CheckIntroducedByDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var customer = new Models.Customer { IntroducedBy = IntroducedBy .Charity};

            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(IntroducedBy.Charity, customer.IntroducedBy);
        }

        [Test]
        public void CustomerTests_CheckReasonForTerminationIsPopulated_WhenSetDefaultValuesIsCalled()
        {
            var customer = new Models.Customer { DateOfTermination = DateTime.UtcNow };

            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(ReasonForTermination.Other, customer.ReasonForTermination);
        }

        [Test]
        public void CustomerTests_CheckDateOfRegistrationIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer{ DateOfRegistration = DateTime.UtcNow };
            var customerPatch = new Models.CustomerPatch { DateOfRegistration = DateTime.MaxValue };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.DateOfRegistration);
        }

        [Test]
        public void CustomerTests_CheckTitleIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { Title = Title.NotProvided };
            var customerPatch = new Models.CustomerPatch { Title = Title.Dr };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(Title.Dr, customer.Title);
        }

        [Test]
        public void CustomerTests_CheckGivenNameIsUpdated_WhenPatchIsCalled()
        {
            var givenName = "John";
            var customer = new Models.Customer { GivenName = "GivenName" };
            var customerPatch = new Models.CustomerPatch { GivenName = givenName };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(givenName, customer.GivenName);
        }

        [Test]
        public void CustomerTests_CheckFamilyNameIsUpdated_WhenPatchIsCalled()
        {
            var familyName = "Smith";
            var customer = new Models.Customer { FamilyName = "FamilyName" };
            var customerPatch = new Models.CustomerPatch { FamilyName = familyName };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(familyName, customer.FamilyName);
        }

        [Test]
        public void CustomerTests_CheckDateofBirthIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { DateofBirth = DateTime.UtcNow };
            var customerPatch = new Models.CustomerPatch { DateofBirth = DateTime.MaxValue };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.DateofBirth);
        }

        [Test]
        public void CustomerTests_CheckGenderIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { Gender = Gender.NotProvided };
            var customerPatch = new Models.CustomerPatch { Gender = Gender.NotApplicable };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(Gender.NotApplicable, customer.Gender);
        }

        [Test]
        public void CustomerTests_CheckUniqueLearnerNumberIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { UniqueLearnerNumber = "0000000000" };
            var customerPatch = new Models.CustomerPatch { UniqueLearnerNumber = "0000000111" };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual("0000000111", customer.UniqueLearnerNumber);
        }

        [Test]
        public void CustomerTests_CheckOptInUserResearchIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { OptInUserResearch = false };
            var customerPatch = new Models.CustomerPatch { OptInUserResearch = true };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(true, customer.OptInUserResearch);
        }

        [Test]
        public void CustomerTests_CheckOptInMarketResearchIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { OptInMarketResearch = false };
            var customerPatch = new Models.CustomerPatch { OptInMarketResearch = true };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(true, customer.OptInMarketResearch);
        }

        [Test]
        public void CustomerTests_CheckDateOfTerminationIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { DateOfTermination = DateTime.UtcNow };
            var customerPatch = new Models.CustomerPatch { DateOfTermination = DateTime.MaxValue };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.DateOfTermination);
        }

        [Test]
        public void CustomerTests_CheckReasonForTerminationIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { ReasonForTermination = ReasonForTermination.Other };
            var customerPatch = new Models.CustomerPatch { ReasonForTermination = ReasonForTermination.Duplicate };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(ReasonForTermination.Duplicate, customer.ReasonForTermination);
        }

        [Test]
        public void CustomerTests_CheckIntroducedByIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { IntroducedBy = IntroducedBy.Other };
            var customerPatch = new Models.CustomerPatch { IntroducedBy = IntroducedBy.NotProvided };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(IntroducedBy.NotProvided, customer.IntroducedBy);
        }

        [Test]
        public void CustomerTests_CheckIntroducedByAdditionalInfoIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { IntroducedByAdditionalInfo = "Info" };
            var customerPatch = new Models.CustomerPatch { IntroducedByAdditionalInfo = "More Info" };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual("More Info", customer.IntroducedByAdditionalInfo);
        }

        [Test]
        public void CustomerTests_CheckLastModifiedDatesUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { LastModifiedDate = DateTime.UtcNow };
            var customerPatch = new Models.CustomerPatch { LastModifiedDate = DateTime.MaxValue };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.LastModifiedDate);
        }


        [Test]
        public void CustomerTests_CheckLastModifiedTouchpointIdIsUpdated_WhenPatchIsCalled()
        {
            var customer = new Models.Customer { LastModifiedTouchpointId = "0000000000" };
            var customerPatch = new Models.CustomerPatch { LastModifiedTouchpointId = "0000000111" };

            customer.Patch(customerPatch);

            // Assert
            Assert.AreEqual("0000000111", customer.LastModifiedTouchpointId);
        }
    }
}
