using DFC.Common.Standard.Logging;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.ChangeFeed.SQLServer;
using System.Collections.Generic;

namespace NCS.DSS.Customer.ChangeFeed.CustomerChangeFeedTrigger.Service
{
    public class CustomerChangeFeedTriggerService : ICustomerChangeFeedTriggerService
    {
        private readonly ILoggerHelper _loggerHelper;
        private readonly ISQLServerProvider _sqlServerProvider;
        public CustomerChangeFeedTriggerService(ILoggerHelper loggerHelper, ISQLServerProvider sqlServerProvider)
        {
            _loggerHelper = loggerHelper;
            _sqlServerProvider = sqlServerProvider;            
        }
        public void PersistChangeAsync(IReadOnlyList<Document> documents, ILogger log)
        {
            _loggerHelper.LogMethodEnter(log);
            
            foreach (Document document in documents)
            {
                _sqlServerProvider.UpsertResource(document, log);
            }

            _loggerHelper.LogMethodExit(log);
        }
    }
}
