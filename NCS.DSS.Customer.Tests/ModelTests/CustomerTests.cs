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
            Assert.That(customer.DateOfRegistration, Is.Not.Null);
            Assert.That(customer.LastModifiedDate, Is.Not.Null);
            Assert.That(customer.OptInUserResearch, Is.False);
            Assert.That(customer.OptInMarketResearch, Is.False);
            Assert.That(customer.Title, Is.EqualTo(Title.NotProvided));
            Assert.That(customer.Gender, Is.EqualTo(Gender.NotProvided));
            Assert.That(customer.IntroducedBy, Is.EqualTo(IntroducedBy.NotProvided));
        }

        [Test]
        public void CustomerTests_CheckDateOfRegistrationDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { DateOfRegistration = DateTime.MaxValue };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.That(customer.DateOfRegistration, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void CustomerTests_CheckLastModifiedDateDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { LastModifiedDate = DateTime.MaxValue };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.That(customer.LastModifiedDate, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void CustomerTests_CheckOptInUserResearchDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { OptInUserResearch = true };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.That(customer.OptInUserResearch, Is.True);
        }

        [Test]
        public void CustomerTests_CheckOptInMarketResearchDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { OptInMarketResearch = true };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.That(customer.OptInMarketResearch, Is.True);
        }

        [Test]
        public void CustomerTests_CheckTitleDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { Title = Title.Mr };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.That(customer.Title, Is.EqualTo(Title.Mr));
        }

        [Test]
        public void CustomerTests_CheckGenderDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { Gender = Gender.Male };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.That(customer.Gender, Is.EqualTo(Gender.Male));
        }

        [Test]
        public void CustomerTests_CheckIntroducedByDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { IntroducedBy = IntroducedBy.Charity };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.That(customer.IntroducedBy, Is.EqualTo(IntroducedBy.Charity));
        }

        [Test]
        public void CustomerTests_CheckReasonForTerminationIsPopulated_WhenSetDefaultValuesIsCalled()
        {
            // Arrange
            var customer = new Models.Customer { DateOfTermination = DateTime.UtcNow };

            // Act
            customer.SetDefaultValues();

            // Assert
            Assert.That(customer.ReasonForTermination, Is.EqualTo(ReasonForTermination.Other));
        }
    }
}
