using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace NCS.DSS.Customer.Cosmos.Provider
{
    public interface IDocumentDBProvider
    {
        bool DoesCustomerResourceExist(Guid customerId);
        Task<bool> DoesCustomerHaveATerminationDate(Guid customerId);

        Task<List<Models.Customer>> SearchAllCustomer(string givenName = null, string familyName = null, string dateofBirth = null,
            string uniqueLearnerNumber = null);

        Task<List<Models.Customer>> GetAllCustomer();
        Task<Models.Customer> GetCustomerByIdAsync(Guid customerId);
        Task<ResourceResponse<Document>> CreateCustomerAsync(Models.Customer customer);
        Task<ResourceResponse<Document>> UpdateCustomerAsync(Models.Customer customer);
    }
}