using Microsoft.Azure.Documents;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.CustomerChangeFeedTrigger.Service
{
    public class CustomerChangeFeedTriggerService : ICustomerChangeFeedTriggerService
    {
        private readonly string _queueName = Environment.GetEnvironmentVariable("ChangeFeedQueueName");
        private readonly string _serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");

        public async Task SendMessageToChangeFeedQueueAsync(Document document)
        {

            if (document == null)
                return;

            var queueClient = new QueueClient(_serviceBusConnectionString, _queueName);

            var changeFeedMessageModel = new ChangeFeedMessageModel()
            {
                Document = document,
                IsCustomer = true
            };

            var msg = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(changeFeedMessageModel)))
            {
                ContentType = "application/json",
                MessageId = document.Id
            };

            await queueClient.SendAsync(msg);
        }

        public class ChangeFeedMessageModel
        {
            public Document Document { get; set; }
            public bool IsCustomer { get; set; }
        }
    }
}
