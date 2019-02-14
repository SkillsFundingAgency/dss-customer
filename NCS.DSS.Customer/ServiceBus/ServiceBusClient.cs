using Microsoft.Azure.ServiceBus;
using NCS.DSS.Customer.Models;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.ServiceBus
{
    
    public static class ServiceBusClient
    {
        public static readonly string QueueName = Environment.GetEnvironmentVariable("QueueName");
        public static readonly string ServiceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");

        public static async Task SendPostMessageAsync(Models.Customer customer, string reqUrl)
        {
            var queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            var messageModel = new MessageModel()
            {
                TitleMessage = "New Customer record {" + customer.CustomerId + "} added at " + DateTime.UtcNow,
                CustomerGuid = customer.CustomerId,
                LastModifiedDate = customer.LastModifiedDate,
                URL = reqUrl + "/" + customer.CustomerId,
                IsNewCustomer = false,
                TouchpointId = customer.LastModifiedTouchpointId
            };

            var msg = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel)))
            {
                ContentType = "application/json",
                MessageId = customer.CustomerId + " " + DateTime.UtcNow
            };

            await queueClient.SendAsync(msg);
        }

        public static async Task SendPatchMessageAsync(CustomerPatch customerPatch, Guid customerId, string reqUrl)
        {
            var queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            var messageModel = new MessageModel
            {
                TitleMessage = "Outcome record modification for {" + customerId + "} at " + DateTime.UtcNow,
                CustomerGuid = customerId,
                LastModifiedDate = customerPatch.LastModifiedDate,
                URL = reqUrl,
                IsNewCustomer = false,
                TouchpointId = customerPatch.LastModifiedTouchpointId
            };

            var msg = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel)))
            {
                ContentType = "application/json",
                MessageId = customerId + " " + DateTime.UtcNow
            };

            await queueClient.SendAsync(msg);

        }

        public class MessageModel
        {
            public string TitleMessage { get; set; }
            public Guid? CustomerGuid { get; set; }
            public DateTime? LastModifiedDate { get; set; }
            public string URL { get; set; }
            public bool IsNewCustomer { get; set; }
            public string TouchpointId { get; set; }
        }

    }
}

