using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public interface ICustomerPatchService
    {
        string Patch(string customerJson, CustomerPatch customerPatch);
    }
}
