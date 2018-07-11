using NCS.DSS.Customer.Cosmos.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Service
{
    class PostCustomerHttpTriggerService : IPostCustomerHttpTriggerService
    {
        public async Task<Models.Customer> CreateNewCustomerAsync(Models.Customer customer)
        {
            if (customer == null)
                return null;

            var CustomerID = Guid.NewGuid();
            customer.CustomerID = CustomerID;

            var documentDbProvider = new DocumentDBProvider();

            var response = await documentDbProvider.CreateCustomerAsync(customer);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic) response.Resource : (Guid?) null;

        }
    }
}
