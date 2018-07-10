using System;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Service
{
    public interface IPostCustomerHttpTriggerService
    {
        Task<Guid?> CreateNewCustomerAsync(Models.Customer customer);
    }
}
