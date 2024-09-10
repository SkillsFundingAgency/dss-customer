﻿namespace NCS.DSS.Customer.PostCustomerHttpTrigger.Service
{
    public interface IPostCustomerHttpTriggerService
    {
        Task<Models.Customer> CreateNewCustomerAsync(Models.Customer customer);
        Task SendToServiceBusQueueAsync(Models.Customer customer, string reqUrl);
    }
}
