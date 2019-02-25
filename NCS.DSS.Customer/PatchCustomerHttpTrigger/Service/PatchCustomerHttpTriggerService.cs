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
        private readonly ICustomerPatchService _customerPatchService;

        public PatchCustomerHttpTriggerService(ICustomerPatchService customerPatchService, IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
            _customerPatchService = customerPatchService;
        }

        public string PatchResource(string customerJson, CustomerPatch customerPatch)
        {
            if (string.IsNullOrEmpty(customerJson))
                return null;

            if (customerPatch == null)
                return null;

            customerPatch.SetDefaultValues();

            return _customerPatchService.Patch(customerJson, customerPatch);
        }

        public async Task<Models.Customer> UpdateCosmosAsync(string customerJson, Guid customerId)
        {
            if (string.IsNullOrEmpty(customerJson))
                return null;

            var response = await _documentDbProvider.UpdateCustomerAsync(customerJson, customerId);

            var responseStatusCode = response?.StatusCode;

            return responseStatusCode == HttpStatusCode.OK ? (dynamic)response.Resource : null;
        }

        public async Task<string> GetCustomerByIdAsync(Guid customerId)
        {
            return await _documentDbProvider.GetCustomerByIdForUpdateAsync(customerId);
        }

        public async Task SendToServiceBusQueueAsync(CustomerPatch customerPatch, Guid customerId, string reqUrl)
        {
            await ServiceBusClient.SendPatchMessageAsync(customerPatch, customerId, reqUrl);
        }

    }
}
