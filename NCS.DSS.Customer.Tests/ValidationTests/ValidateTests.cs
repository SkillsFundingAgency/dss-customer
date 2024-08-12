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
            var customer = new Models.Customer { IntroducedBy = IntroducedBy.CareersFairActivity };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            //Changed to 5 as PriorityGroups are now required
            Assert.That(result.Count, Is.EqualTo(5));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenGivenNameIsNotPopulatedForPost()
        {
            // Arrange
            var customer = new Models.Customer { FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenFamilyNameIsNotPopulatedForPost()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", IntroducedBy = IntroducedBy.CareersFairActivity, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateOfTerminationNotPopulatedButReasonForTerminationHasAValueForPost()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, ReasonForTermination = ReasonForTermination.CustomerChoice, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenUniqueLearnerNumberIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, UniqueLearnerNumber = "10000000000", PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void ValidateTests_ReturnNoValidationResult_WhenUniqueLearnerNumberIsValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, UniqueLearnerNumber = "5000000000", PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateOfRegistrationIsInTheFuture()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, DateOfRegistration = DateTime.MaxValue, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenUserIsYoungerThan13()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, DateofBirth = DateTime.UtcNow.AddYears(-12), PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenTitleIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, Title = (Title)100, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenGenderIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, Gender = (Gender)100, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenReasonForTerminationIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, ReasonForTermination = (ReasonForTermination)100, DateOfTermination = DateTime.Now.AddDays(-1), PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenIntroducedByIsNotValid()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = (IntroducedBy)100, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateOfTerminationIsInTheFuture()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, DateOfTermination = DateTime.MaxValue, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenLastModifiedDateIsInTheFuture()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = IntroducedBy.CareersFairActivity, LastModifiedDate = DateTime.MaxValue, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, false);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenIntroducedByIsNotPopulatedForPost()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }


        [Test]
        public void ValidateTests_ReturnValidationResult_WhenNullIntroducedByIsNotUpdated()
        {
            // Arrange
            var customer = new Models.Customer { GivenName = "John", FamilyName = "Smith", IntroducedBy = null, PriorityGroups = new List<PriorityCustomer> { PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months } };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenGivenNameFamilyNameNorIntroducedByAreNotPopulatedForPost()
        {
            // Arrange
            var customer = new Models.Customer { };

            // Act
            var result = _validate.ValidateResource(customer, true);

            // Assert
            Assert.That(result, Is.InstanceOf<List<ValidationResult>>());
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(7));
        }
    }
}
