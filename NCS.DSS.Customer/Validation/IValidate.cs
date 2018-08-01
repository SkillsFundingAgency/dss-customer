using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.Validation
{
    public interface IValidate
    {
        List<ValidationResult> ValidateResource(ICustomer resource, bool validateModelForPost);
    }
}