using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Search;
using NCS.DSS.Customer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Cosmos.Provider
{
    public interface IDocumentDBProvider
    {
        Task<bool> DoesCustomerResourceExist(Guid customerId);
        Task<bool> DoesCustomerHaveATerminationDate(Guid customerId);

        Task<List<Models.Customer>> SearchCustomer(ISearchIndexClient indexClient, string searchText,
            string filter = null, IList<string> order = null, IList<string> facets = null);

        Task<List<Models.Customer>> GetAllCustomer();
        Task<Models.Customer> GetCustomerByIdAsync(Guid customerId);
        Task<string> GetCustomerByIdForUpdateAsync(Guid customerId);
        Task<ResourceResponse<Document>> CreateCustomerAsync(Models.Customer customer);
        Task<ResourceResponse<Document>> UpdateCustomerAsync(string customerJson, Guid customerId);

        Task<List<Models.Subscriptions>> GetSubscriptionsByCustomerIdAsync(Guid? customerId);
        Task<ResourceResponse<Document>> CreateSubscriptionsAsync(Models.Subscriptions subscriptions);
        Task<DigitalIdentity> GetIdentityForCustomerAsync(Guid customerId);
        Task<DigitalIdentity> UpdateIdentityAsync(DigitalIdentity digitalIdentity);
    }
}