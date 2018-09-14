using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.Cosmos.Helper
{
    public interface ISubscriptionHelper
    {
        Task<Subscriptions> CreateSubscriptionAsync(Models.Customer customer);
        Task<List<Subscriptions>> GetSubscriptionsAsync(Guid? customerGuid);
    }
}