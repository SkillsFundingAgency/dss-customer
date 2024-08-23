﻿using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public interface IPatchCustomerHttpTriggerService
    {
        string PatchResource(string customerJson, CustomerPatch customerPatch);
        Task<Models.Customer> UpdateCosmosAsync(string customerJson, Guid customerId);
        Task<string> GetCustomerByIdAsync(Guid customerId);
        Task SendToServiceBusQueueAsync(Models.CustomerPatch customerPatch, Guid customerId, string reqUrl);
    }
}
