using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NCS.DSS.Customer.Validation
{
    public class Validate : IValidate
    {
        public List<ValidationResult> ValidateResource(Models.Customer customer)
        {
            var context = new ValidationContext(customer, null, null);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(customer, context, results, true);

            return isValid ? null : results;
        }
    }
}
