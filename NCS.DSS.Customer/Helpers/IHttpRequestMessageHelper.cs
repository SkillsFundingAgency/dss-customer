using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Helpers
{
    public interface IHttpRequestMessageHelper
    {
        Task<Models.Customer> GetCustomerFromRequest(HttpRequestMessage req);
    }
}