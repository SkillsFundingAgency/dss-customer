using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.Cosmos.Helper
{
    public class SubscriptionHelper : ISubscriptionHelper
    {
        private readonly IDocumentDBProvider _documentDbProvider;
        public SubscriptionHelper(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }

        public async Task<Subscriptions> CreateSubscriptionAsync(Models.Customer customer)
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

            var response = await _documentDbProvider.CreateSubscriptionsAsync(subscription);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic)response.Resource : (Guid?)null;
        }

        public async Task<List<Subscriptions>> GetSubscriptionsAsync(Guid? customerGuid)
        {            
            var subscriptions = await _documentDbProvider.GetSubscriptionsByCustomerIdAsync(customerGuid);

            return subscriptions;
        }

    }
}
