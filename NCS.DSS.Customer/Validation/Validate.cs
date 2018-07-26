using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.ReferenceData;

namespace NCS.DSS.Customer.Validation
{
    public class Validate : IValidate
    {
        public List<ValidationResult> ValidateResource(ICustomer resource)
        {
            var context = new ValidationContext(resource, null, null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(resource, context, results, true);

            ValidateCustomerRules(resource, results);

            return results;
        }

        private void ValidateCustomerRules(ICustomer customerResource, List<ValidationResult> results)
        {
            if (customerResource == null)
                return;

            if (customerResource.DateOfRegistration.HasValue && customerResource.DateOfRegistration.Value > DateTime.Now)
                results.Add(new ValidationResult("Date of Registration must be less the current date", new[] { "DateOfRegistration" }));

            if (customerResource.DateofBirth.HasValue && customerResource.DateofBirth.Value > DateTime.Now.AddYears(-13))
                results.Add(new ValidationResult("Customer must be at least 13 years old to use this service.", new[] { "DateofBirth" }));

            if (customerResource.Title.HasValue && !Enum.IsDefined(typeof(Title), customerResource.Title.Value))
                results.Add(new ValidationResult("Please supply a valid Title", new[] { "Title" }));

            if (customerResource.Gender.HasValue && !Enum.IsDefined(typeof(Gender), customerResource.Gender.Value))
                results.Add(new ValidationResult("Please supply a valid Gender", new[] { "Gender" }));

            if(customerResource.DateOfTermination == null && customerResource.ReasonForTermination.HasValue)
                results.Add(new ValidationResult("Please enter a Termination Date", new[] { "DateOfTermination" }));

            if (customerResource.ReasonForTermination.HasValue && !Enum.IsDefined(typeof(ReasonForTermination), customerResource.ReasonForTermination.Value))
                results.Add(new ValidationResult("Please supply a valid Reason For Termination", new[] { "ReasonForTermination" }));

            if (customerResource.IntroducedBy.HasValue && !Enum.IsDefined(typeof(IntroducedBy), customerResource.IntroducedBy.Value))
                results.Add(new ValidationResult("Please supply a valid Introduced By value", new[] { "IntroducedBy" }));
        }

    }
}
