using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public interface ICustomerPatchService
    {
        Models.Customer Patch(Models.Customer customer, CustomerPatch customerPatch);
    }
}
