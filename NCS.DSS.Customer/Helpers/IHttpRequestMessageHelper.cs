using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Helpers
{
    public interface IHttpRequestMessageHelper
    {
        Task<T> GetCustomerFromRequest<T>(HttpRequestMessage req);
        string GetTouchpointId(HttpRequestMessage req);
    }
}