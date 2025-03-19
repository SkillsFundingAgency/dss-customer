using Azure.Search.Documents.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Helpers;
using System.Text.Json;

namespace NCS.DSS.Customer.AzureSearchDataSyncTrigger
{
    public class CustomerSearchDataSyncTrigger
    {
        private readonly ILogger<CustomerSearchDataSyncTrigger> _logger;
        public CustomerSearchDataSyncTrigger( ILogger<CustomerSearchDataSyncTrigger> logger)
        {
            _logger = logger;
        }

        [Function("SyncDataForCustomerSearchTrigger")]
        public async Task Run(
            [CosmosDBTrigger("customers", "customers", ConnectionStringSetting = "CustomerConnectionString",
                LeaseCollectionName = "customers-leases", CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Models.CustomerDocument> documents)
        {
            _logger.LogInformation("{functionName} started",nameof(CustomerSearchDataSyncTrigger));

            _logger.LogInformation("Attempting get Search Service Client");

            var client = SearchHelper.GetSearchServiceClient();

            _logger.LogInformation("Number of Documents modified in Cosmos DB : {count}",documents.Count);

            if (documents.Count > 0)
            { 
                try
                {      
                    var customers = new List<Models.CustomerSearch>();
                    foreach (var doc in documents)
                    {
                        var custJson = JsonSerializer.Serialize(doc);
                        var customer = JsonSerializer.Deserialize<Models.CustomerSearch>(custJson);
                        if(doc.CustomerId == null && doc.id != null)
                            customer.CustomerId = doc.id;
                        else if(doc.id == null && doc.CustomerId != null)
                            customer.CustomerId = doc.CustomerId;

                        customers.Add(customer);
                    }
                    var custFiltered = customers.Where(d => d.CustomerId != null);
                    if (custFiltered.Any())
                    {

                        _logger.LogInformation("Attempting to Merge / Upload documents with IDs ({Ids}) to azure search", string.Join(',', custFiltered.Select(d => d.CustomerId).ToArray()));
                        var batch = IndexDocumentsBatch.MergeOrUpload(custFiltered);

                        _logger.LogInformation("Attempting to Index documents to azure search");
                        var results = await client.IndexDocumentsAsync(batch);

                        var failed = results.Value.Results.Where(r => !r.Succeeded).Select(r => r.Key).ToList();

                        if (failed.Count > 0)
                        {
                            _logger.LogWarning("Failed to Index some of the documents: {errors}", string.Join(", ", failed));
                        }
                        else { 
                            _logger.LogInformation("Successfully Merged and Indexed documnets to azure search");
                        }                        
                    }
                    var custFailed = customers.Where(d => d.CustomerId == null);
                    if (custFailed.Any())
                    {
                        foreach (var doc in custFailed.Where(d => d.CustomerId == null))
                        {
                            _logger.LogWarning("{Doc} missing Document Key (CustomerId) ", JsonSerializer.Serialize(doc));
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e,"Request failed Excpetion with {error}", e.Message);
                    throw;
                }               
            } 
            _logger.LogInformation("{functionName} exited", nameof(CustomerSearchDataSyncTrigger));
        }
    }
}