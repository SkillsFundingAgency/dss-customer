using System;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public interface IPatchCustomerHttpTriggerService
    {
        Task<Models.Customer> UpdateCustomerAsync(Models.Customer customer, Models.CustomerPatch customerPatch);
        Task<Models.Customer> GetCustomerByIdAsync(Guid customerId);
        Task SendToServiceBusQueueAsync(Models.CustomerPatch customerPatch, Guid customerId, string reqUrl);
    }
}
