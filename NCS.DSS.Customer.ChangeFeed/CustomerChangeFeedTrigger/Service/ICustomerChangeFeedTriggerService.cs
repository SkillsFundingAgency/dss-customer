using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.ChangeFeed.CustomerChangeFeedTrigger.Service
{
    public interface ICustomerChangeFeedTriggerService
    {
        Task PersistChangeAsync(IReadOnlyList<Document> documents, ILogger log);
    }
}
