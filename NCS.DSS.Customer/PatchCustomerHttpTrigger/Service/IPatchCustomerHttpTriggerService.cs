using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public interface IPatchCustomerHttpTriggerService
    {
        Task<Models.Customer> UpdateCustomerAsync(Models.Customer customer, Models.CustomerPatch customerPatch);
        Task<Models.Customer> GetCustomerByIdAsync(Guid customerId);

    }
}
