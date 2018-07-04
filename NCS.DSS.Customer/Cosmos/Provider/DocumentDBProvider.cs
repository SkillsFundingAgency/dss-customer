using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NCS.DSS.Customer.Cosmos.Client;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Cosmos.Provider;

namespace NCS.DSS.Customer.Cosmos.Provider
{
    public class DocumentDBProvider : IDocumentDBProvider
    {
        private readonly DocumentDBHelper _documentDbHelper;
        private readonly DocumentDBClient _databaseClient;

        public DocumentDBProvider()
        {
            _documentDbHelper = new DocumentDBHelper();
            _databaseClient = new DocumentDBClient();
        }

        public bool DoesCustomerResourceExist(Guid customerId)
        {
            var collectionUri = _documentDbHelper.CreateDocumentCollectionUri();

            var client = _databaseClient.CreateDocumentClient();

            if (client == null)
                return false;

            var query = client.CreateDocumentQuery<Models.Customer>(collectionUri, new FeedOptions { MaxItemCount = 1 });
            var customerExists = query.Where(x => x.CustomerID == customerId).AsEnumerable().Any();

            return customerExists;
        }

        public async Task<ResourceResponse<Document>> GetCustomerAsync(Guid customerId)
        {
            var documentUri = _documentDbHelper.CreateDocumentUri(customerId);

            var client = _databaseClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.ReadDocumentAsync(documentUri);

            return response;
        }
                
        public async Task<ResourceResponse<Document>> CreateCustomerAsync(Models.Customer customer)
        {

            var collectionUri = _documentDbHelper.CreateDocumentCollectionUri();

            var client = _databaseClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.CreateDocumentAsync(collectionUri, customer);

            return response;

        }

        public async Task<ResourceResponse<Document>> UpdateCustomerAsync(Models.Customer customer)
        {
            var documentUri = _documentDbHelper.CreateDocumentUri(customer.CustomerID);

            var client = _databaseClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.ReplaceDocumentAsync(documentUri, customer);

            return response;
        }
    }
}