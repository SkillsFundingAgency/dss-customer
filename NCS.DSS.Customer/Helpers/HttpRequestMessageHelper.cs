using System.Net.Http;
using System.Threading.Tasks;
using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.Helpers
{
    public class HttpRequestMessageHelper : IHttpRequestMessageHelper
    {
        public async Task<T> GetCustomerFromRequest<T>(HttpRequestMessage req)
        {
            return await req.Content.ReadAsAsync<T>();
        }
    }
}
