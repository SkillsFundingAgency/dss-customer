using NCS.DSS.Customer.ReferenceData;
using NCS.DSS.Customer.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NCS.DSS.Customer.Tests.ValidationTests
{
    [TestFixture]
    public class ValidateTests
    {
        private IValidate _validate;

        [SetUp]
        public void Setup()
        {
            _validate = new Validate();
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenGivenNameAndFamilyNameIsNotPopulatedForPost()
        {
            // Arrange
            var customer = new Models.Customer();

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            //Changed to 5 as PriorityGroups are now required
            Assert.AreEqual(5, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenGivenNameIsNotPopulatedForPost()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenFamilyNameIsNotPopulatedForPost()
        {
            // Arrange
            var customer = new Models.Customer { FamilyName = "Smith", PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateOfTerminationNotPopulatedButReasonForTerminationHasAValueForPost()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", ReasonForTermination = ReasonForTermination.CustomerChoice, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenUniqueLearnerNumberIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", UniqueLearnerNumber = "10000000000", PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnNoValidationResult_WhenUniqueLearnerNumberIsValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", UniqueLearnerNumber = "5000000000", PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateOfRegistrationIsInTheFuture()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", DateOfRegistration = DateTime.MaxValue, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenUserIsYoungerThan13()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", DateofBirth = DateTime.UtcNow.AddYears(-12), PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenTitleIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", Title = (Title)100, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenGenderIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", Gender = (Gender)100, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenReasonForTerminationIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", ReasonForTermination = (ReasonForTermination)100, DateOfTermination = DateTime.Now.AddDays(-1), PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenIntroducedByIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = (IntroducedBy)100, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateOfTerminationIsInTheFuture()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", DateOfTermination = DateTime.MaxValue, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenLastModifiedDateIsInTheFuture()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", LastModifiedDate = DateTime.MaxValue, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

    }
}