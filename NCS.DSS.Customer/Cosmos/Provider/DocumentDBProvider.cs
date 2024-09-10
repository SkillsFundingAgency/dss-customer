using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using NCS.DSS.Customer.Cosmos.Client;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Models;
using Newtonsoft.Json.Linq;
using System.Net;
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

            return customers.Any() ? customers : null;
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
                    return (dynamic)response.Resource;
            }
            catch (DocumentClientException)
            {
                return null;
            }

            return null;
        }

        public async Task<string> GetCustomerByIdForUpdateAsync(Guid customerId)
        {
            var documentUri = DocumentDBHelper.CreateDocumentUri(customerId);

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            try
            {
                var response = await client.ReadDocumentAsync(documentUri);
                if (response.Resource != null)
                    return response.Resource.ToString();
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

        public async Task<ResourceResponse<Document>> UpdateCustomerAsync(string customerJson, Guid customerId)
        {
            if (string.IsNullOrEmpty(customerJson))
                return null;

            var documentUri = DocumentDBHelper.CreateDocumentUri(customerId);

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var customerDocumentObject = JObject.Parse(customerJson);

            var response = await client.ReplaceDocumentAsync(documentUri, customerDocumentObject);

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

        public async Task<DigitalIdentity> GetIdentityForCustomerAsync(Guid customerId)
        {
            var collectionUri = DocumentDBHelper.CreateDigitalIdentityDocumentUri();
            var client = DocumentDBClient.CreateDocumentClient();

            var identityForCustomerQuery = client
                ?.CreateDocumentQuery<Models.DigitalIdentity>(collectionUri, new FeedOptions { MaxItemCount = 1 })
                .Where(x => x.CustomerId == customerId)
                .AsDocumentQuery();

            if (identityForCustomerQuery == null)
                return null;

            var digitalIdentity = await identityForCustomerQuery.ExecuteNextAsync<Models.DigitalIdentity>();

            return digitalIdentity?.FirstOrDefault();
        }

        public async Task<Models.DigitalIdentity> UpdateIdentityAsync(Models.DigitalIdentity digitalIdentity)
        {
            var documentUri = DocumentDBHelper.CreateDigitalIdentityDocumentUri(digitalIdentity.IdentityID.GetValueOrDefault());

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.ReplaceDocumentAsync(documentUri, digitalIdentity);

            return response.StatusCode == HttpStatusCode.OK ? (dynamic)response.Resource : null;
        }
    }
}