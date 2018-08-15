using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.ReferenceData;

namespace NCS.DSS.Customer.Validation
{
    public class Validate : IValidate
    {
        public List<ValidationResult> ValidateResource(ICustomer resource, bool validateModelForPost)
        {
            var context = new ValidationContext(resource, null, null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(resource, context, results, true);

            ValidateCustomerRules(resource, results, validateModelForPost);

            return results;
        }

        private void ValidateCustomerRules(ICustomer customerResource, List<ValidationResult> results, bool validateModelForPost)
        {
            if (customerResource == null)
                return;

            if (validateModelForPost)
            {
                if (string.IsNullOrWhiteSpace(customerResource.FamilyName))
                    results.Add(new ValidationResult("Family Name is a required field", new[] { "FamilyName" }));

                if (string.IsNullOrWhiteSpace(customerResource.GivenName))
                    results.Add(new ValidationResult("Given Name is a required field", new[] { "GivenName" }));

                if (customerResource.DateOfTermination == null && customerResource.ReasonForTermination.HasValue)
                    results.Add(new ValidationResult("Please enter a Termination Date", new[] { "DateOfTermination" }));
            }

            if (long.TryParse(customerResource.UniqueLearnerNumber, out var uln))
            {
                if(uln < 1000000000 && uln > 9999999999)
                    results.Add(new ValidationResult("Unique Learner Number must be greater than 1000000000 and less than 9999999999", 
                        new[] { "UniqueLearnerNumber" }));
            }

            if (customerResource.DateOfRegistration.HasValue && customerResource.DateOfRegistration.Value > DateTime.UtcNow)
                results.Add(new ValidationResult("Date of Registration must be less the current date/time", new[] { "DateOfRegistration" }));

            if (customerResource.DateofBirth.HasValue && customerResource.DateofBirth.Value > DateTime.UtcNow.AddYears(-13))
                results.Add(new ValidationResult("Customer must be at least 13 years old to use this service.", new[] { "DateofBirth" }));

            if (customerResource.Title.HasValue && !Enum.IsDefined(typeof(Title), customerResource.Title.Value))
                results.Add(new ValidationResult("Please supply a valid Title", new[] { "Title" }));

            if (customerResource.Gender.HasValue && !Enum.IsDefined(typeof(Gender), customerResource.Gender.Value))
                results.Add(new ValidationResult("Please supply a valid Gender", new[] { "Gender" }));

            if (customerResource.DateOfTermination.HasValue && customerResource.DateOfTermination.Value > DateTime.UtcNow)
                results.Add(new ValidationResult("Date Of Termination must be less the current date/time", new[] { "DateOfTermination" }));

            if (customerResource.LastModifiedDate.HasValue && customerResource.LastModifiedDate.Value > DateTime.UtcNow)
                results.Add(new ValidationResult("Last Modified Date must be less the current date/time", new[] { "LastModifiedDate" }));

            if (customerResource.ReasonForTermination.HasValue && !Enum.IsDefined(typeof(ReasonForTermination), customerResource.ReasonForTermination.Value))
                results.Add(new ValidationResult("Please supply a valid Reason For Termination", new[] { "ReasonForTermination" }));

            if (customerResource.IntroducedBy.HasValue && !Enum.IsDefined(typeof(IntroducedBy), customerResource.IntroducedBy.Value))
                results.Add(new ValidationResult("Please supply a valid Introduced By value", new[] { "IntroducedBy" }));
        }

    }
}
