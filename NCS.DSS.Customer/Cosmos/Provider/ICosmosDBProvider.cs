using Microsoft.Azure.Cosmos;
using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.Cosmos.Provider
{
    public interface ICosmosDBProvider
    {
        Task<bool> DoesCustomerResourceExist(Guid customerId);
        Task<bool> DoesCustomerHaveATerminationDate(Guid customerId);
        Task<List<Models.Customer>> GetAllCustomer();
        Task<Models.Customer> GetCustomerByIdAsync(Guid customerId);
        Task<string> GetCustomerByIdForUpdateAsync(Guid customerId);
        Task<ItemResponse<Models.Customer>> CreateCustomerAsync(Models.Customer customer);
        Task<ItemResponse<Models.Customer>> UpdateCustomerAsync(string customerJson, Guid customerId);
        Task<List<Subscriptions>> GetSubscriptionsByCustomerIdAsync(Guid? customerId);
        Task<Subscriptions> CreateSubscriptionsAsync(Models.Customer customer);
        Task<DigitalIdentity> GetIdentityForCustomerAsync(Guid customerId);
        Task<DigitalIdentity> UpdateIdentityAsync(DigitalIdentity digitalIdentity);
    }
}