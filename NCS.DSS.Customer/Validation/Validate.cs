using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NCS.DSS.Customer.Validation
{
    public class Validate : IValidate
    {
        public List<ValidationResult> ValidateResource<T>(T resource)
        {
            var context = new ValidationContext(resource, null, null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(resource, context, results, true);

            return results;
        }
    }
}
