using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.Models;
using Newtonsoft.Json;
using System.Text;

namespace NCS.DSS.Customer.ServiceBus
{
    public class CustomerServiceBusClient : ICustomerServiceBusClient
    {
        private readonly ICosmosDBProvider _cosmosDBProvider;
        private readonly ILogger<CustomerServiceBusClient> _logger;
        public readonly string QueueName = Environment.GetEnvironmentVariable("QueueName");
        public readonly string ServiceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
        private readonly ServiceBusClient _serviceBusClient;

        public CustomerServiceBusClient(ICosmosDBProvider cosmosDBProvider, ServiceBusClient serviceBusClient, ILogger<CustomerServiceBusClient> logger)
        {
            _cosmosDBProvider = cosmosDBProvider;
            _serviceBusClient = serviceBusClient;
            _logger = logger;
        }

        public async Task SendPostMessageAsync(Models.Customer customer, string reqUrl)
        {
            try
            {
                _logger.LogInformation("Attempting to Create Sender for Service Bus Client");
                var serviceBusSender = _serviceBusClient.CreateSender(QueueName);
                _logger.LogInformation("Preparing Message for Service Bus");
                var messageModel = new MessageModel()
                {
                    TitleMessage = "New Customer record {" + customer.CustomerId + "} added at " + DateTime.UtcNow,
                    CustomerGuid = customer.CustomerId,
                    LastModifiedDate = customer.LastModifiedDate,
                    URL = reqUrl + "/" + customer.CustomerId,
                    IsNewCustomer = false,
                    TouchpointId = customer.LastModifiedTouchpointId
                };

                var msg = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel)))
                {
                    ContentType = "application/json",
                    MessageId = customer.CustomerId + " " + DateTime.UtcNow
                };

                _logger.LogInformation("Attempting to Auto Subscribe Customer with ID {CustomerID}",customer.CustomerId);
                await AutoSubscribeCustomer(customer);

                _logger.LogInformation("Attempting to Send Service Bus Message for Customer with ID {CustomerID}", customer.CustomerId);
                await serviceBusSender.SendMessageAsync(msg);
                _logger.LogInformation("Service Bus Message for Customer with ID {CustomerID} has been sent successfully", customer.CustomerId);

            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed to Send Service Bus Message for Customer with ID {CustomerID}. Exception Raised with {Message}. {StackTrace}", customer.CustomerId, ex.Message,ex.StackTrace);
                throw;
            }            
        }

        public async Task SendPatchMessageAsync(CustomerPatch customerPatch, Guid customerId, string reqUrl)
        {
            try
            {
                _logger.LogInformation("Attempting to Create Sender for Service Bus Client");
                var serviceBusSender = _serviceBusClient.CreateSender(QueueName);
                _logger.LogInformation("Preparing Message for Service Bus");

                var messageModel = new MessageModel
                {
                    TitleMessage = "Outcome record modification for {" + customerId + "} at " + DateTime.UtcNow,
                    CustomerGuid = customerId,
                    LastModifiedDate = customerPatch.LastModifiedDate,
                    URL = reqUrl,
                    IsNewCustomer = false,
                    TouchpointId = customerPatch.LastModifiedTouchpointId,
                    IsDigitalAccount = customerPatch.IsDigitalAccount,
                    UpdateDigitalIdentity = customerPatch.UpdateDigitalIdentity,
                    FirstName = customerPatch.GivenName,
                    LastName = customerPatch.FamilyName,
                    IdentityStoreId = customerPatch.IdentityStoreId,
                    DeleteDigitalIdentity = customerPatch.DeleteDigitalIdentity
                };

                var msg = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel)))
                {
                    ContentType = "application/json",
                    MessageId = customerId + " " + DateTime.UtcNow
                };

                _logger.LogInformation("Attempting to Send Service Bus Message for Customer with ID {CustomerID}", customerId);
                await serviceBusSender.SendMessageAsync(msg);
                _logger.LogInformation("Service Bus Message for Customer with ID {CustomerID} has been sent successfully", customerId);


            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed to Send Service Bus Message for Customer with ID {CustomerID}. Exception Raised with {Message}. {StackTrace}", customerId, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public class MessageModel
        {
            public string TitleMessage { get; set; }
            public Guid? CustomerGuid { get; set; }
            public DateTime? LastModifiedDate { get; set; }
            public string URL { get; set; }
            public bool IsNewCustomer { get; set; }
            public string TouchpointId { get; set; }
            public bool? IsDigitalAccount { get; set; }
            public bool? UpdateDigitalIdentity { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Guid? IdentityStoreId { get; set; }
            public bool? DeleteDigitalIdentity { get; set; }
        }

        private async Task AutoSubscribeCustomer(Models.Customer customer)
        {
            //Auto subscribe last modified touchpoint to the newly posted customer
            await _cosmosDBProvider.CreateSubscriptionsAsync(customer);
        }

    }
}

