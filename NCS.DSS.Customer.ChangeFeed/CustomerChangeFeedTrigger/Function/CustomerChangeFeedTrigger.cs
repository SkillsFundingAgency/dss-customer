using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.ChangeFeed.CustomerChangeFeedTrigger.Service;
using System.Collections.Generic;

namespace NCS.DSS.Customer.ChangeFeed.CustomerChangeFeedTrigger.Function
{
    public static class CustomerChangeFeedTrigger
    {
        private const string _databaseName = "collections";
        private const string _collectionName = "customers";
        private const string _connectionString = "CustomersConnString";

        [FunctionName("CustomerChangeFeedTrigger")]        
        public static void Run([CosmosDBTrigger(
            databaseName: _databaseName,
            collectionName: _collectionName,
            ConnectionStringSetting = _connectionString,
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log,
            [Inject]ICustomerChangeFeedTriggerService changeFeedService,
            [Inject]ILoggerHelper loggerHelper)
        {
            loggerHelper.LogMethodEnter(log);

            changeFeedService.PersistChangeAsync(input, log);

            loggerHelper.LogMethodExit(log);
        }        
    }
}
