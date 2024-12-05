using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCS.DSS.Customer.Models;
using System.Net;
using System.Text.Json;
using Container = Microsoft.Azure.Cosmos.Container;

namespace NCS.DSS.Customer.Cosmos.Provider
{
    public class CosmosDBProvider : ICosmosDBProvider
    {
        private readonly Container _container;
        private readonly Container _subscriptionContainer;
        private readonly Container _digialIdentityContainer;
        private readonly ILogger<CosmosDBProvider> _logger;
        public CosmosDBProvider(CosmosClient cosmosClient,
            IOptions<CustomerConfigurationSettings> configOptions,
            ILogger<CosmosDBProvider> logger)
        {
            var config = configOptions.Value;
            _container = GetContainer(cosmosClient, config.DatabaseId,config.CollectionId);
            _subscriptionContainer = GetContainer(cosmosClient, config.SubscriptionDatabaseId, config.SubscriptionCollectionId); 
            _digialIdentityContainer = GetContainer(cosmosClient, config.DigitalIdentityDatabaseId, config.DigitalIdentityCollectionId);
            _logger = logger;
        }
        private static Container GetContainer(CosmosClient cosmosClient, string databaseId, string collectionId)
            => cosmosClient.GetContainer(databaseId, collectionId);
        public async Task<bool> DoesCustomerResourceExist(Guid customerId)
        {
            try
            {
                var queryCust = _container.GetItemLinqQueryable<Models.Customer>().Where(x => x.CustomerId == customerId).ToFeedIterator();

                while (queryCust.HasMoreResults)
                {
                    var response = await queryCust.ReadNextAsync();
                    if (response != null)
                    {
                        _logger.LogInformation("Customer Record found in Cosmos DB for {CustomerID}", customerId);
                        return true;
                    }                    
                }
                _logger.LogWarning("No Customer Record found with {CustomerID} in Cosmos DB", customerId);
                return false;
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to find the Customer Record in Cosmos DB {CustomerID}. Exception {Exception}.", customerId, ce.Message);
                throw;
            }
            
        }

        public async Task<bool> DoesCustomerHaveATerminationDate(Guid customerId)
        {
            try
            {
                var queryCust = _container.GetItemLinqQueryable<Models.Customer>().Where(x => x.CustomerId == customerId).ToFeedIterator();

                while (queryCust.HasMoreResults)
                {
                    var response = await queryCust.ReadNextAsync();    
                    var tDate = response.Resource.FirstOrDefault().DateOfTermination;
                    _logger.LogInformation("Customer with {CustomerID} Have a termination date of {tDate} ", customerId,tDate);
                    return tDate.HasValue;
                }
                _logger.LogWarning("No Customer Record found with {CustomerID} in Cosmos DB", customerId);
                return false;
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to get DateOfTermination for {CustomerID}. Exception {Exception}.", customerId, ce.Message);
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
                _logger.LogInformation("Number of Customer Records found {Count} ", customers.Count);

                return customers.Count != 0 ? customers : null;
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to get Customer data. Exception {Exception}", ce.Message);
                
                throw;
            }
        }

        public async Task<Models.Customer> GetCustomerByIdAsync(Guid customerId)
        {
            try
            {
                var queryCust = _container.GetItemLinqQueryable<Models.Customer>(). Where(x => x.CustomerId == customerId).ToFeedIterator();

                while (queryCust.HasMoreResults)
                {
                    var response = await queryCust.ReadNextAsync();
                    if (response != null)
                    {
                        _logger.LogInformation("Customer Record found in Cosmos DB for {CustomerID}", customerId);
                        return response.Resource.FirstOrDefault();
                    }                    
                }
                _logger.LogWarning("No Customer Record found with {CustomerID} in Cosmos DB", customerId);
                return null;
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to find the Customer Record in Cosmos DB {CustomerID}. Exception {Exception}.", customerId, ce.Message);
                throw;
            }            
        }

        public async Task<string> GetCustomerByIdForUpdateAsync(Guid customerId)
        {
            try
            {
                var queryCust = _container.GetItemLinqQueryable<Models.Customer>().Where(x => x.CustomerId == customerId).ToFeedIterator();

                while (queryCust.HasMoreResults)
                {
                    var response = await queryCust.ReadNextAsync();
                    if (response != null)
                    {
                        var customerJson = JsonSerializer.Serialize(response.Resource.FirstOrDefault());
                        _logger.LogInformation("Customer Record found in Cosmos DB for {CustomerID}", customerId);
                        return customerJson;
                    }
                }
                _logger.LogWarning("No Customer Record found with {CustomerID} in Cosmos DB", customerId);
                return null;
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to find the Customer Record for update in Cosmos DB {CustomerID}. Exception {Exception}.", customerId, ce.Message);
                throw;
            }
        }

        public async Task<ItemResponse<Models.Customer>> CreateCustomerAsync(Models.Customer customer)
        {
            try
            {
                var response = await _container.CreateItemAsync(customer, null);
                if (response.StatusCode == HttpStatusCode.Created)
                { 
                    _logger.LogInformation("Customer Record Created in Cosmos DB for {CustomerID}", customer.CustomerId);
                }
                else
                {
                    _logger.LogError("Failed and returned {StatusCode} to Create Customer Record in Cosmos DB for {CustomerID}", response.StatusCode, customer.CustomerId);
                }
                return response;
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to Create Customer Record in Cosmos DB {CustomerID}. Exception {Exception}.",customer.CustomerId, ce.Message);
                throw;
            }
        }

        public async Task<ItemResponse<Models.Customer>> UpdateCustomerAsync(string customerJson, Guid customerId)
        {
            try
            {
                var customer = JsonSerializer.Deserialize<Models.Customer>(customerJson);
                var response = await _container.ReplaceItemAsync(customer, customerId.ToString());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("Customer Record Updated in Cosmos DB for {CustomerID}", customer.CustomerId);
                }
                else
                {
                    _logger.LogError("Failed and returned {StatusCode} to Update Customer Record in Cosmos DB for {CustomerID}",response.StatusCode, customer.CustomerId);
                }                    
                return response;
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to Update Customer Record in Cosmos DB {CustomerID}. Exception {Exception}.", customerId, ce.Message);
                throw;
            }           
        }

        public async Task<Subscriptions> CreateSubscriptionsAsync(Models.Customer customer)
        {
            if (customer == null)
            {
                return null;
            }                
            try
            {
                var subscription = new Subscriptions
                {
                    SubscriptionId = Guid.NewGuid(),
                    CustomerId = customer.CustomerId,
                    TouchPointId = customer.LastModifiedTouchpointId,
                    Subscribe = true,
                    LastModifiedDate = customer.LastModifiedDate,
                };

                if (!customer.LastModifiedDate.HasValue)
                    subscription.LastModifiedDate = DateTime.Now;
                var response = await _subscriptionContainer.CreateItemAsync(subscription, null);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    _logger.LogInformation("Subscription Record Created in Cosmos DB for Customer with ID {CustomerID}", customer.CustomerId);
                    return response.Resource;
                }
                else
                {
                    _logger.LogError("Failed to Create Subscription Record in Cosmos DB for Customer with ID {CustomerID}. Response Code {StatusCode}. ", customer.CustomerId, response.StatusCode);
                    return null;
                }                                    
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to Create Subscription Record in Cosmos DB for Customer with ID {CustomerID}. Exception {Exception}.", customer.CustomerId, ce.Message);
                throw;
            }
        }

        public async Task<DigitalIdentity> GetIdentityForCustomerAsync(Guid customerId)
        {
            try
            {
                var query = _digialIdentityContainer.GetItemLinqQueryable<DigitalIdentity>()
                                .Where(x => x.CustomerId == customerId)
                                .ToFeedIterator();
                if (query == null)
                    return null;
                if (query.HasMoreResults)
                {
                    var digitalIdentity = await query.ReadNextAsync();
                    _logger.LogInformation("Digital Identity Record found in Cosmos DB for Customer with ID {CustomerID}", customerId);
                    return digitalIdentity?.FirstOrDefault();
                }
                else
                {
                    _logger.LogError("Failed to Retrieve Digital Identity Record in Cosmos DB for Customer with ID {CustomerID}", customerId);
                    return null;
                }
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to Retrieve Digital Identity Record in Cosmos DB for Customer with ID {CustomerID}. Exception {Exception}.", customerId, ce.Message);
                throw;
            }
            
        }

        public async Task<DigitalIdentity> UpdateIdentityAsync(DigitalIdentity digitalIdentity)
        {
            try
            {
                var response = await _digialIdentityContainer.ReplaceItemAsync(digitalIdentity, digitalIdentity.IdentityID.ToString());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("Digital Identity Record Updated in Cosmos DB for {DigiID}", digitalIdentity.IdentityID);
                }
                else
                {
                    _logger.LogInformation("Failed and returned {StatusCode} to Update Digital Identity Record in Cosmos DB for {DigiID}", response.StatusCode, digitalIdentity.IdentityID);
                }
                return response.Resource;
            }
            catch (CosmosException ce)
            {
                _logger.LogError(ce,"Failed to Update Digital Identity Record in Cosmos DB with ID {DigId}. Exception {Exception}.", digitalIdentity.IdentityID, ce.Message);

                throw;
            }
        }
    }
}