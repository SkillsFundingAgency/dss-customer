using DFC.Common.Standard.Logging;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.ChangeFeed.SQLServer;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task PersistChangeAsync(IReadOnlyList<Document> documents, ILogger log)
        {
            _loggerHelper.LogMethodEnter(log);
            
            foreach (Document document in documents)
            {
                await _sqlServerProvider.UpsertResource(document, log);
            }

            _loggerHelper.LogMethodExit(log);
        }
    }
}
