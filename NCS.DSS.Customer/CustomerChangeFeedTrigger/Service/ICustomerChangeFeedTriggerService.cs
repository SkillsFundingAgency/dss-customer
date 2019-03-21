using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.CustomerChangeFeedTrigger.Service
{
    public interface ICustomerChangeFeedTriggerService
    {
        Task SendMessageToChangeFeedQueueAsync(Document document);
    }
}
