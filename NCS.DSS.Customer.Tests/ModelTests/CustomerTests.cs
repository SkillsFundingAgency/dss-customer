using NCS.DSS.Customer.ReferenceData;
using NUnit.Framework;
using System;

namespace NCS.DSS.Customer.Tests.ModelTests
{
    [TestFixture]
    public class CustomerTests
    {
        [Test]
        public void CustomerTests_PopulatesDefaultValues_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer();

            // Act
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
            // Arrange
            var customer = new Models.Customer { DateOfRegistration = DateTime.MaxValue };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.DateOfRegistration);
        }

        [Test]
        public void CustomerTests_CheckLastModifiedDateDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { LastModifiedDate = DateTime.MaxValue };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(DateTime.MaxValue, customer.LastModifiedDate);
        }

        [Test]
        public void CustomerTests_CheckOptInUserResearchDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { OptInUserResearch = true };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(true, customer.OptInUserResearch);
        }

        [Test]
        public void CustomerTests_CheckOptInMarketResearchDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { OptInMarketResearch = true };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(true, customer.OptInMarketResearch);
        }

        [Test]
        public void CustomerTests_CheckTitleDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { Title = Title.Mr };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(Title.Mr, customer.Title);
        }

        [Test]
        public void CustomerTests_CheckGenderDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { Gender = Gender.Male };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(Gender.Male, customer.Gender);
        }

        [Test]
        public void CustomerTests_CheckIntroducedByDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { IntroducedBy = IntroducedBy.Charity };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(IntroducedBy.Charity, customer.IntroducedBy);
        }

        [Test]
        public void CustomerTests_CheckReasonForTerminationIsPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { DateOfTermination = DateTime.UtcNow };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.AreEqual(ReasonForTermination.Other, customer.ReasonForTermination);
        }
    }
}
