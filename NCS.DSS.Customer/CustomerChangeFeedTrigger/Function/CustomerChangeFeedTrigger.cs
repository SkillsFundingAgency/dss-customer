using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.CustomerChangeFeedTrigger.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.CustomerChangeFeedTrigger.Function
{
    public static class CustomerChangeFeedTrigger
    {
        private const string _databaseName = "%DatabaseName%";
        private const string _collectionName = "%CollectionId%";
        private const string _connectionString = "CustomerConnectionString";
        private const string _leaseCollectionName = "%LeaseCollectionName%";
        private const string _leaseCollectionPrefix = "%LeaseCollectionPrefix%";

        [FunctionName("CustomerChangeFeedTrigger")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: _databaseName,
            collectionName: _collectionName,
            ConnectionStringSetting = _connectionString,
            LeaseCollectionName = _leaseCollectionName,
            LeaseCollectionPrefix = _leaseCollectionPrefix,
            CreateLeaseCollectionIfNotExists = true
            )]IReadOnlyList<Document> documents, ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]ICustomerChangeFeedTriggerService customerChangeFeedTriggerService)
        {
            loggerHelper.LogMethodEnter(log);

            await customerChangeFeedTriggerService.PersistChangeAsync(documents, log);

            loggerHelper.LogMethodExit(log);
        }
    }
}
