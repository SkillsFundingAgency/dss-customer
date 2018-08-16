using System;
using NCS.DSS.Customer.Cosmos.Provider;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.Customer.ServiceBus;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Service
{
    public class PostCustomerHttpTriggerService : IPostCustomerHttpTriggerService
    {
        public async Task<Models.Customer> CreateNewCustomerAsync(Models.Customer customer)
        {
            if (customer == null)
                return null;

            customer.SetDefaultValues();

            var documentDbProvider = new DocumentDBProvider();

            var response = await documentDbProvider.CreateCustomerAsync(customer);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic) response.Resource : null;

        }

        public async Task SendToServiceBusQueueAsync(Models.Customer customer, Guid customerId, string reqUrl)
        {
            await ServiceBusClient.SendPostMessageAsync(customer, customerId, reqUrl);
        }

    }
}
