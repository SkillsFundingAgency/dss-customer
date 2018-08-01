using System;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Cosmos.Provider
{
    public interface IDocumentDBProvider
    {
        Task<Models.Customer> GetCustomerByIdAsync(Guid customerId);

    }
}