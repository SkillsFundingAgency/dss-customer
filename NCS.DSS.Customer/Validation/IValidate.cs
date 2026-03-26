using NCS.DSS.Customer.Models;
using System.ComponentModel.DataAnnotations;

namespace NCS.DSS.Customer.Validation
{
    public interface IValidate
    {
        List<ValidationResult> ValidateResource(ICustomer resource, bool validateModelForPost);
    }
}