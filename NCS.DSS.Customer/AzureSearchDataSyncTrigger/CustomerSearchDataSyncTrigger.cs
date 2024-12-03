using Azure;
using Azure.Search.Documents.Models;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.ReferenceData;
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
        public async Task RunAsync(
            [CosmosDBTrigger("customers", "customers", ConnectionStringSetting = "CustomerConnectionString",
                LeaseCollectionName = "customers-leases", CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Models.CustomerSearch> documents)
        {
            _logger.LogInformation("{functionName} started",nameof(CustomerSearchDataSyncTrigger));

            var client = SearchHelper.GetSearchServiceClient();

            _logger.LogInformation("get search service client");


            _logger.LogInformation("get index client");

            _logger.LogInformation("Documents modified {count}",documents.Count);

            if (documents.Count > 0)
            { 
                try
                {      
                    var customers = new List<Models.CustomerSearch>();
                    foreach (var doc in documents)
                    {
                        var customer = doc;
                        if(doc.CustomerId == null && doc.id != null)
                            customer.CustomerId = doc.id;
                        if(doc.id == null && doc.CustomerId != null)
                            customer.id = doc.CustomerId;
                        customers.Add(customer);
                    }
                    var custFiltered = customers.Where(d => d.CustomerId != null && d.id != null);
                    if (custFiltered.Any())
                    {
                        var batch = IndexDocumentsBatch.MergeOrUpload(custFiltered);

                        _logger.LogInformation("attempting to merge docs to azure search");

                        _logger.LogInformation("Document IDs : {Ids}", string.Join(',', custFiltered.Select(d => d.CustomerId).ToArray()));


                        var results = await client.IndexDocumentsAsync(batch);

                        var failed = results.Value.Results.Where(r => !r.Succeeded).Select(r => r.Key).ToList();

                        if (failed.Count > 0)
                        {
                            _logger.LogInformation("Failed to index some of the documents: {errors}", string.Join(", ", failed));
                        }

                        _logger.LogInformation("successfully merged docs to azure search");
                    }
                    var custFailed = customers.Where(d => d.CustomerId == null && d.id == null);
                    if (custFailed.Any())
                    {
                        _logger.LogInformation("Below list of documents can't be processed as they are missing with Document Key");
                        foreach (var doc in custFailed.Where(d => d.CustomerId == null))
                        {
                            _logger.LogInformation(JsonSerializer.Serialize(doc));
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Request failed Excpetion with {error} {stacktrace}", e.Message, e.StackTrace);
                    throw;
                }               
            } 
            _logger.LogInformation("{functionName} exited", nameof(CustomerSearchDataSyncTrigger));
        }
    }
}