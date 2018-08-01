using NCS.DSS.Customer.Cosmos.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger.Service
{
    class SearchCustomerHttpTriggerService : ISearchCustomerHttpTriggerService
    {
        public async Task<List<Models.Customer>> SearchCustomerAsync(string qQuery)
        {
            var documentDbProvider = new DocumentDBProvider();
            var customer = await documentDbProvider.SearchAllCustomer(qQuery);

            return customer;
        }





    }
}
