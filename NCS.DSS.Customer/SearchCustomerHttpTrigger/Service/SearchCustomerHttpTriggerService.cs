using NCS.DSS.Customer.Cosmos.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger.Service
{
    public class SearchCustomerHttpTriggerService : ISearchCustomerHttpTriggerService
    {
        public async Task<List<Models.Customer>> SearchCustomerAsync(string givenName = null, string familyName = null, string dateofBirth = null,
            string uniqueLearnerNumber = null)
        {
            var documentDbProvider = new DocumentDBProvider();
            var customer = await documentDbProvider.SearchAllCustomer(givenName, familyName, dateofBirth, uniqueLearnerNumber);

            return customer;
        }
    }
}
