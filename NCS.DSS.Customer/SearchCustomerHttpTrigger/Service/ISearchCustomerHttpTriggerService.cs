using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger.Service
{
    public interface ISearchCustomerHttpTriggerService
    {
        Task<List<Models.Customer>> SearchCustomerAsync(ISearchIndexClient indexClient, string searchText,
            string filter = null, IList<string> order = null, IList<string> facets = null);
    }
}