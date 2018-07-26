using NCS.DSS.Customer.Cosmos.Provider;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service 
{
    public class PatchCustomerHttpTriggerService : IPatchCustomerHttpTriggerService
    {
        public async Task<Models.Customer> UpdateCustomerAsync(Models.Customer customer, Models.CustomerPatch customerPatch)
        {
            if (customer == null)
                return null;

            customerPatch.SetDefaultValues();

            customer.Patch(customerPatch);

            var documentDbProvider = new DocumentDBProvider();
            var response = await documentDbProvider.UpdateCustomerAsync(customer);

            var responseStatusCode = response.StatusCode;

            return responseStatusCode == HttpStatusCode.OK ? customer : null;
        }

        public async Task<Models.Customer> GetCustomerByIdAsync(Guid customerId)
        {
            var documentDbProvider = new DocumentDBProvider();
            var customer = await documentDbProvider.GetCustomerByIdAsync(customerId);

            return customer;
        }


    }
}
