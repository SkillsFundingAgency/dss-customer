using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.ChangeFeed.CustomerChangeFeedTrigger.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.ChangeFeed.CustomerChangeFeedTrigger.Function
{
    public static class CustomerChangeFeedTrigger
    {
        private const string _databaseName = "customers";
        private const string _collectionName = "customers";
        private const string _connectionString = "CustomerConnectionString";
        private const string _leaseCollectionName = "customers-lease";
        private const string _collectionsPrefix = "customersChangeFeedPrefix";        

        [FunctionName("CustomerChangeFeedTrigger")]        
        public static async Task Run([CosmosDBTrigger(
            databaseName: _databaseName,
            collectionName: _collectionName,
            ConnectionStringSetting = _connectionString,
            LeaseCollectionName = _leaseCollectionName,
            LeaseCollectionPrefix = _collectionsPrefix,
            CreateLeaseCollectionIfNotExists = true
            )]IReadOnlyList<Document> input, ILogger log,
            [Inject]ICustomerChangeFeedTriggerService changeFeedService,
            [Inject]ILoggerHelper loggerHelper)
        {
            loggerHelper.LogMethodEnter(log);

            await changeFeedService.PersistChangeAsync(input, log);

            loggerHelper.LogMethodExit(log);
        }        
    }
}
