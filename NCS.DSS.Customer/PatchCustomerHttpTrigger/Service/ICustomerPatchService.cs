using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public interface ICustomerPatchService
    {
        Models.Customer Patch(string customerJson, CustomerPatch customerPatch);
    }
}
