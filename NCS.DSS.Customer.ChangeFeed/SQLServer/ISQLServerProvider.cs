using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;

namespace NCS.DSS.Customer.ChangeFeed.SQLServer
{
    public interface ISQLServerProvider
    {
        bool UpsertResource(Document document, ILogger log);
    }
}
