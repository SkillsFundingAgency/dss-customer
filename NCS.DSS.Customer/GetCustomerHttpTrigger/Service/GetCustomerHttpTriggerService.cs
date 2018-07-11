using NCS.DSS.Customer.Cosmos.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.GetCustomerHttpTrigger.Service
{
    class GetCustomerHttpTriggerService : IGetCustomerHttpTriggerService
    {
        public async Task<List<Models.Customer>> GetAllCustomerAsync()
        {
            var documentDbProvider = new DocumentDBProvider();
            var customer = documentDbProvider.GetAllCustomer();

            return await customer;
        }





    }
}
