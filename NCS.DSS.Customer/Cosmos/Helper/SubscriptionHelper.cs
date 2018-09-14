using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customers.Cosmos.Helper
{
    public class SubscriptionHelper : ISubscriptionHelper
    {
        public async Task<Subscriptions> CreateSubscriptionAsync(Customer.Models.Customer customer)
        {
            if (customer == null)
                return null;

            var subscription = new Subscriptions
            {
                SubscriptionId = Guid.NewGuid(),
                CustomerId = customer.CustomerId,
                TouchPointId = customer.LastModifiedTouchpointId,
                Subscribe = true,
                LastModifiedDate = customer.LastModifiedDate,

            };

            if (!customer.LastModifiedDate.HasValue)
                subscription.LastModifiedDate = DateTime.Now;

            var documentDbProvider = new DocumentDBProvider();

            var response = await documentDbProvider.CreateSubscriptionsAsync(subscription);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic)response.Resource : (Guid?)null;
        }

        public async Task<List<Subscriptions>> GetSubscriptionsAsync(Guid? customerGuid)
        {
            var documentDbProvider = new DocumentDBProvider();
            var subscriptions = await documentDbProvider.GetSubscriptionsByCustomerIdAsync(customerGuid);

            return subscriptions;
        }

    }
}
