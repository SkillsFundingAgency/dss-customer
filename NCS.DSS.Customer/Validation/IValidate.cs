using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NCS.DSS.Customer.Validation
{
    public interface IValidate
    {
        List<ValidationResult> ValidateResource(Models.Customer customer);
    }
}