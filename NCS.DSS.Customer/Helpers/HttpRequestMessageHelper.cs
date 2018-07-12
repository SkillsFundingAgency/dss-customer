using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.Helpers
{
    public class HttpRequestMessageHelper : IHttpRequestMessageHelper
    {
        public async Task<T> GetCustomerFromRequest<T>(HttpRequestMessage req)
        {
            req.Content.Headers.ContentType.MediaType = "application/json";
            return await req.Content.ReadAsAsync<T>();
        }
    }
}
