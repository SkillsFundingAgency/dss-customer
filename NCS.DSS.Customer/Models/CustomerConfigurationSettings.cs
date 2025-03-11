using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Models
{
    public class CustomerConfigurationSettings
    {
        public required string CosmosDbEndpoint { get; set; }
        public required string CustomerConnectionString { get; set; }
        public required string QueueName { get; set; }
        public required string ServiceBusConnectionString { get; set; }
        public required string DatabaseId { get; set; }
        public required string CollectionId { get; set; }
        public required string DigitalIdentityDatabaseId { get; set; }
        public required string DigitalIdentityCollectionId { get; set; }
        public required string SubscriptionCollectionId { get; set; }
        public required string SubscriptionDatabaseId { get; set; }
    }
}
