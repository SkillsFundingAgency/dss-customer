using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.ServiceBus;
using System.Net;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Service
{
    public class PostCustomerHttpTriggerService : IPostCustomerHttpTriggerService
    {
        private readonly ICosmosDBProvider _documentDbProvider;
        private readonly ICustomerServiceBusClient _serviceBusClient;

        public PostCustomerHttpTriggerService(ICosmosDBProvider documentDbProvider, ICustomerServiceBusClient serviceBusClient)
        {
            _documentDbProvider = documentDbProvider;
            _serviceBusClient = serviceBusClient;
        }
        public async Task<Models.Customer> CreateNewCustomerAsync(Models.Customer customer)
        {
            if (customer == null)
                return null;

            customer.SetDefaultValues();

            var response = await _documentDbProvider.CreateCustomerAsync(customer);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic)response.Resource : null;

        }

        public async Task SendToServiceBusQueueAsync(Models.Customer customer, string reqUrl)
        {
            await _serviceBusClient.SendPostMessageAsync(customer, reqUrl);
        }

    }
}
