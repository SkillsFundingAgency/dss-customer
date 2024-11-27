using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Models;
using System.Text.Json;
using Container = Microsoft.Azure.Cosmos.Container;

namespace NCS.DSS.Customer.Cosmos.Provider
{
    public class DocumentDBProvider : IDocumentDBProvider
    {
        private readonly Container _container;
        private readonly string _databaseId = Environment.GetEnvironmentVariable("DatabaseId");
        private readonly string _containerId = Environment.GetEnvironmentVariable("CollectionId");
        private readonly ILogger<DocumentDBProvider> _logger;
        public DocumentDBProvider(CosmosClient cosmosClient,ILogger<DocumentDBProvider> logger)
        {
            _container = cosmosClient.GetContainer(_databaseId, _containerId);
            _logger = logger;
        }
        public async Task<bool> DoesCustomerResourceExist(Guid customerId)
        {
            try
            {
                ItemResponse<Models.Customer> response = await _container.ReadItemAsync<Models.Customer>(
                    partitionKey: new PartitionKey(customerId.ToString()),
                    id: customerId.ToString());

                return response.DocumentId() > 0;
            }
            catch (CosmosException ce)
            {
                _logger.LogError("Failed to find the Customer Record in Cosmos DB {CustomerID}. Exception {Exception}", customerId, ce.Message);
                throw;
            }
            
        }

        public async Task<bool> DoesCustomerHaveATerminationDate(Guid customerId)
        {
            try
            {
                ItemResponse<Models.Customer> response = await _container.ReadItemAsync<Models.Customer>(
                    partitionKey: new PartitionKey(customerId.ToString()),
                    id: customerId.ToString());

                return response.Resource.DateOfTermination.HasValue;
            }
            catch (CosmosException ce)
            {
                _logger.LogError("Failed to get DateOfTermination for {CustomerID}. Exception {Exception}", customerId, ce.Message);
                throw;
            }
        }

        public async Task<List<Models.Customer>> GetAllCustomer()
        {
            try
            {
                var queryCust = _container.GetItemLinqQueryable<Models.Customer>().ToFeedIterator();
                var customers = new List<Models.Customer>();

                while (queryCust.HasMoreResults)
                {
                    var response = await queryCust.ReadNextAsync();
                    customers.AddRange(response);
                }

                return customers.Count != 0 ? customers : null;
            }
            catch (CosmosException ce)
            {
                _logger.LogError("Failed to get Customer data. Exception {Exception}", ce.Message);
                
                throw;
            }
        }

        public async Task<Models.Customer> GetCustomerByIdAsync(Guid customerId)
        {
            try
            {
                ItemResponse<Models.Customer> response = await _container.ReadItemAsync<Models.Customer>(
                    partitionKey: new PartitionKey(customerId.ToString()),
                    id: customerId.ToString());

                return response.DocumentId() > 0 ? response.Resource : null;
            }
            catch (CosmosException ce)
            {
                _logger.LogError("Failed to find the Customer Record in Cosmos DB {CustomerID}. Exception {Exception}", customerId, ce.Message);
                throw;
            }            
        }

        public async Task<string> GetCustomerByIdForUpdateAsync(Guid customerId)
        {
            try
            {
                ItemResponse<Models.Customer> response = await _container.ReadItemAsync<Models.Customer>(
                    partitionKey: new PartitionKey(customerId.ToString()),
                    id: customerId.ToString());

                return response.DocumentId() > 0 ? response.Resource.ToString() : null;
            }
            catch (CosmosException ce)
            {
                _logger.LogError("Failed to find the Customer Record for update in Cosmos DB {CustomerID}. Exception {Exception}", customerId, ce.Message);
                throw;
            }
        }

        public async Task<ItemResponse<Models.Customer>> CreateCustomerAsync(Models.Customer customer)
        {
            return await _container.CreateItemAsync(customer, new PartitionKey(customer.CustomerId.ToString()));

        }

        public async Task<ItemResponse<Models.Customer>> UpdateCustomerAsync(string customerJson, Guid customerId)
        {
            var customer = JsonSerializer.Deserialize<Models.Customer>(customerJson);
            return await _container.ReplaceItemAsync(customer, customerId.ToString());
        }

        public async Task<List<Models.Subscriptions>> GetSubscriptionsByCustomerIdAsync(Guid? customerId)
        {
            var query = _container.GetItemLinqQueryable<Models.Subscriptions>()
                .Where(x => x.CustomerId == customerId &&
                            x.Subscribe)
                .ToFeedIterator();

            if (query == null)
                return null;

            var subscriptions = new List<Models.Subscriptions>();

            while (query.HasMoreResults)
            {
                var results = await query.ReadNextAsync();
                subscriptions.AddRange(results);
            }

            return subscriptions.Count != 0 ? subscriptions : null;
        }

        public async Task<ItemResponse<Models.Subscriptions>> CreateSubscriptionsAsync(Models.Subscriptions subscriptions)
        {
            return await _container.CreateItemAsync(subscriptions, new PartitionKey(subscriptions.CustomerId.ToString()));     
        }

        public async Task<DigitalIdentity> GetIdentityForCustomerAsync(Guid customerId)
        {
            var query = _container.GetItemLinqQueryable<Models.DigitalIdentity>()
                .Where(x => x.CustomerId == customerId)
                .ToFeedIterator();

            if (query == null)
                return null;
            if (query.HasMoreResults)
            {
                var digitalIdentity = await query.ReadNextAsync();

                return digitalIdentity?.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public async Task<Models.DigitalIdentity> UpdateIdentityAsync(Models.DigitalIdentity digitalIdentity)
        {
            return await _container.ReplaceItemAsync(digitalIdentity, digitalIdentity.IdentityID.ToString());
        }
    }
}