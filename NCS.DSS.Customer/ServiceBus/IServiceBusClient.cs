using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.ServiceBus
{
    public interface IServiceBusClient
    {
        Task SendPostMessageAsync(Models.Customer customer, string reqUrl);
        Task SendPatchMessageAsync(CustomerPatch customerPatch, Guid customerId, string reqUrl);
    }
}
