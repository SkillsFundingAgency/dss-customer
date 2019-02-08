using NCS.DSS.Customer.Cosmos.Provider;
using System;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.ServiceBus;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service 
{
    public class PatchCustomerHttpTriggerService : IPatchCustomerHttpTriggerService
    {
        private readonly IDocumentDBProvider _documentDbProvider;

        public PatchCustomerHttpTriggerService(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }
        public async Task<Models.Customer> UpdateCustomerAsync(Models.Customer customer, Models.CustomerPatch customerPatch)
        {
            if (customer == null)
                return null;

            customerPatch.SetDefaultValues();

            customer.Patch(customerPatch);
            
            var response = await _documentDbProvider.UpdateCustomerAsync(customer);

            var responseStatusCode = response.StatusCode;

            return responseStatusCode == HttpStatusCode.OK ? customer : null;
        }

        public async Task<Models.Customer> GetCustomerByIdAsync(Guid customerId)
        {
            return await _documentDbProvider.GetCustomerByIdAsync(customerId);
        }

        public async Task SendToServiceBusQueueAsync(CustomerPatch customerPatch, Guid customerId, string reqUrl)
        {
            await ServiceBusClient.SendPatchMessageAsync(customerPatch, customerId, reqUrl);
        }
    }
}
