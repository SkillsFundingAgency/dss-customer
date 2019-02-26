using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.CustomerChangeFeedTrigger.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.CustomerChangeFeedTrigger.Function
{
    public static class CustomerChangeFeedTrigger
    {
        private const string DatabaseName = "%DatabaseName%";
        private const string CollectionName = "%CollectionId%";
        private const string ConnectionString = "CustomerConnectionString";
        private const string LeaseCollectionName = "%LeaseCollectionName%";
        private const string LeaseCollectionPrefix = "%LeaseCollectionPrefix%";

        [FunctionName("CustomerChangeFeedTrigger")]
        public static async Task Run([CosmosDBTrigger(
            DatabaseName,
            CollectionName,
            ConnectionStringSetting = ConnectionString,
            LeaseCollectionName = LeaseCollectionName,
            LeaseCollectionPrefix = LeaseCollectionPrefix,
            CreateLeaseCollectionIfNotExists = true
            )]IReadOnlyList<Document> documents, ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]ICustomerChangeFeedTriggerService customerChangeFeedTriggerService)
        {
            loggerHelper.LogMethodEnter(log);

            try
            {
                foreach (var document in documents)
                {
                    loggerHelper.LogInformationMessage(log, Guid.NewGuid(), string.Format("Attempting to send document id: {0} to service bus queue", document.Id));
                    await customerChangeFeedTriggerService.SendMessageToChangeFeedQueueAsync(document);
                }
            }
            catch (Exception ex)
            {
                loggerHelper.LogException(log, Guid.NewGuid(), "Error when trying to add document to service bus queue", ex);
            }

            loggerHelper.LogMethodExit(log);
        }
    }
}
