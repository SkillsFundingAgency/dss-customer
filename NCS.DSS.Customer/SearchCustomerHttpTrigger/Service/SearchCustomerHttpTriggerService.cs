using NCS.DSS.Customer.Cosmos.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger.Service
{
    public class SearchCustomerHttpTriggerService : ISearchCustomerHttpTriggerService
    {
        public async Task<List<Models.Customer>> SearchCustomerAsync(ISearchIndexClient indexClient, string searchText,
            string filter = null, IList<string> order = null, IList<string> facets = null)
        {
            var documentDbProvider = new DocumentDBProvider();
            var customer = await documentDbProvider.SearchCustomer(indexClient, searchText, filter, order, facets);

            return customer;
        }
    }
}
