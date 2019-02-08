using DFC.AzureSql.Standard;
using DFC.Common.Standard.Logging;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.CustomerChangeFeedTrigger.Service
{
    public class CustomerChangeFeedTriggerService : ICustomerChangeFeedTriggerService
    {
        private readonly ILoggerHelper _loggerHelper;
        private readonly ISQLServerProvider _sqlServerProvider;
        private static readonly string CommandText = Environment.GetEnvironmentVariable("SQLCommandText");
        private static readonly string ParameterName = Environment.GetEnvironmentVariable("SQLParameterName");
        public CustomerChangeFeedTriggerService(ILoggerHelper loggerHelper, ISQLServerProvider sqlServerProvider)
        {
            _loggerHelper = loggerHelper;
            _sqlServerProvider = sqlServerProvider;
        }
        public async Task PersistChangeAsync(IReadOnlyList<Document> documents, ILogger log)
        {
            _loggerHelper.LogMethodEnter(log);

            try
            {
                foreach (Document document in documents)
                {
                    await _sqlServerProvider.UpsertResource(document, log, CommandText, ParameterName);
                }
            }
            catch (Exception ex)
            {
                _loggerHelper.LogException(log, Guid.NewGuid(), "Error when trying to upsert into SQL", ex);
            }

            _loggerHelper.LogMethodExit(log);
        }
    }
}
