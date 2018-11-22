using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using NCS.DSS.Customer.Cosmos.Client;
using NCS.DSS.Customer.Cosmos.Helper;
using Document = Microsoft.Azure.Documents.Document;

namespace NCS.DSS.Customer.Cosmos.Provider
{
    public class DocumentDBProvider : IDocumentDBProvider
    {
        public async Task<bool> DoesCustomerResourceExist(Guid customerId)
        {
            var documentUri = DocumentDBHelper.CreateDocumentUri(customerId);

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return false;

            try
            {
                var response = await client.ReadDocumentAsync(documentUri);
                if (response.Resource != null)
                    return true;
            }
            catch (DocumentClientException)
            {
                return false;
            }

            return false;
        }

        public async Task<bool> DoesCustomerHaveATerminationDate(Guid customerId)
        {
            var documentUri = DocumentDBHelper.CreateDocumentUri(customerId);

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return false;

            try
            {
                var response = await client.ReadDocumentAsync(documentUri);

                var dateOfTermination = response.Resource?.GetPropertyValue<DateTime?>("DateOfTermination");

                return dateOfTermination.HasValue;
            }
            catch (DocumentClientException)
            {
                return false;
            }
        }

        public async Task<List<Models.Customer>> SearchCustomer(ISearchIndexClient indexClient, string searchText,
            string filter = null, IList<string> order = null, IList<string> facets = null)
        {
            var sp = new SearchParameters
            {
                QueryType = QueryType.Full,
                SearchMode = SearchMode.All,
                IncludeTotalResultCount = true,
                Top = 1000
            };

            //Add Filter
            if (!string.IsNullOrEmpty(filter))
            {
                sp.Filter = filter;
            }

            //Order
            if (order != null && order.Count > 0)
            {
                sp.OrderBy = order;
            }

            //facets
            if (facets != null && facets.Count > 0)
            {
                sp.Facets = facets;
            }

            //Search
            var response = await indexClient.Documents.SearchAsync<Models.Customer>(searchText, sp);

            return response.Results.Select(result => result.Document).ToList();

        }

        public async Task<List<Models.Customer>> GetAllCustomer()
        {
            var collectionUri = DocumentDBHelper.CreateDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var queryCust = client.CreateDocumentQuery<Models.Customer>(collectionUri).AsDocumentQuery();

            var customers = new List<Models.Customer>();

            while (queryCust.HasMoreResults)
            {
                var response = await queryCust.ExecuteNextAsync<Models.Customer>();
                customers.AddRange(response);
            }

            return customers.Any() ? customers: null;
        }
        
        public async Task<Models.Customer> GetCustomerByIdAsync(Guid customerId)
        {
            var documentUri = DocumentDBHelper.CreateDocumentUri(customerId);

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            try
            {
                var response = await client.ReadDocumentAsync(documentUri);
                if (response.Resource != null)
                    return (dynamic) response.Resource;
            }
            catch (DocumentClientException)
            {
                return null;
            }

            return null;
        }
                
        public async Task<ResourceResponse<Document>> CreateCustomerAsync(Models.Customer customer)
        {
            var collectionUri = DocumentDBHelper.CreateDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.CreateDocumentAsync(collectionUri, customer);

            return response;

        }

        public async Task<ResourceResponse<Document>> UpdateCustomerAsync(Models.Customer customer)
        {
            var documentUri = DocumentDBHelper.CreateDocumentUri(customer.CustomerId);

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.ReplaceDocumentAsync(documentUri, customer);

            return response;
        }
        
        public async Task<List<Models.Subscriptions>> GetSubscriptionsByCustomerIdAsync(Guid? customerId)
        {
            var collectionUri = DocumentDBHelper.CreateSubscriptionDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            var query = client
                ?.CreateDocumentQuery<Models.Subscriptions>(collectionUri)
                .Where(x => x.CustomerId == customerId &&
                            x.Subscribe)
                .AsDocumentQuery();

            if (query == null)
                return null;

            var subscriptions = new List<Models.Subscriptions>();

            while (query.HasMoreResults)
            {
                var results = await query.ExecuteNextAsync<Models.Subscriptions>();
                subscriptions.AddRange(results);
            }

            return subscriptions.Any() ? subscriptions : null;
        }

        public async Task<ResourceResponse<Document>> CreateSubscriptionsAsync(Models.Subscriptions subscriptions)
        {
            var collectionUri = DocumentDBHelper.CreateSubscriptionDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.CreateDocumentAsync(collectionUri, subscriptions);

            return response;

        }
        
    }
}