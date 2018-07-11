using System;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Service
{
    public interface IPostCustomerHttpTriggerService
    {
        Task<Models.Customer> CreateNewCustomerAsync(Models.Customer customer);
    }
}
