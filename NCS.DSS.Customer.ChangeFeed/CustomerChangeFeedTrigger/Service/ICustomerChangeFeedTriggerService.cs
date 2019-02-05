using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace NCS.DSS.Customer.ChangeFeed.CustomerChangeFeedTrigger.Service
{
    public interface ICustomerChangeFeedTriggerService
    {
        void PersistChangeAsync(IReadOnlyList<Document> documents, ILogger log);
    }
}
