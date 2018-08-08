using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Cosmos.Provider
{
    public interface IDocumentDBProvider
    {
        Task<Models.Customer> GetCustomerByIdAsync(Guid customerId);

        Task<List<Models.Customer>> SearchAllCustomer(string givenName = null, string familyName = null, string dateofBirth = null,
            string uniqueLearnerNumber = null);
    }
}