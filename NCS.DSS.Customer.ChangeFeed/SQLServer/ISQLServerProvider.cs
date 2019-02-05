using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.ChangeFeed.SQLServer
{
    public interface ISQLServerProvider
    {
        Task<bool> UpsertResource(Document document, ILogger log);
    }
}
