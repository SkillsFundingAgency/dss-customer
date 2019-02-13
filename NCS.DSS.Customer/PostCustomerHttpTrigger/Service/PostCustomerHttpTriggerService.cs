using System;
using NCS.DSS.Customer.Cosmos.Provider;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.Customer.ServiceBus;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Service
{
    public class PostCustomerHttpTriggerService : IPostCustomerHttpTriggerService
    {
        private readonly IDocumentDBProvider _documentDbProvider;
        public PostCustomerHttpTriggerService(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }
        public async Task<Models.Customer> CreateNewCustomerAsync(Models.Customer customer)
        {
            if (customer == null)
                return null;

            customer.SetDefaultValues();            

            var response = await _documentDbProvider.CreateCustomerAsync(customer);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic) response.Resource : null;

        }

        public async Task SendToServiceBusQueueAsync(Models.Customer customer, string reqUrl)
        {
            await ServiceBusClient.SendPostMessageAsync(customer, reqUrl);
        }

    }
}
