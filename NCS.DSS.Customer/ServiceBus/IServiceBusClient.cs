using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.ServiceBus
{
    public interface IServiceBusClient
    {
        Task SendPostMessageAsync(Models.Customer customer, string reqUrl);
        Task SendPatchMessageAsync(CustomerPatch customerPatch, Guid customerId, string reqUrl);
    }
}
