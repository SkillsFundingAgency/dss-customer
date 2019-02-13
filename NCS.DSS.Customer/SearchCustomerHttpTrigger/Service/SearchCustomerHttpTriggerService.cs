using NCS.DSS.Customer.Cosmos.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger.Service
{
    public class SearchCustomerHttpTriggerService : ISearchCustomerHttpTriggerService
    {
        private readonly IDocumentDBProvider _documentDbProvider;
        public SearchCustomerHttpTriggerService(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }
        public async Task<List<Models.Customer>> SearchCustomerAsync(ISearchIndexClient indexClient, string searchText,
            string filter = null, IList<string> order = null, IList<string> facets = null)
        {             
            return await _documentDbProvider.SearchCustomer(indexClient, searchText, filter, order, facets);            
        }
    }
}
