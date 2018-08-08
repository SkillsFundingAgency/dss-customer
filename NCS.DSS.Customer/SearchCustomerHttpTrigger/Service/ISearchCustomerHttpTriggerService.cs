using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger.Service
{
    public interface ISearchCustomerHttpTriggerService
    {
        Task<List<Models.Customer>> SearchCustomerAsync(string givenName = null, string familyName = null, string dateofBirth = null, string uniqueLearnerNumber = null);
    }
}
