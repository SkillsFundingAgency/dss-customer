using System.Text.Json;
using Azure;
using Azure.Search.Documents.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Helpers;
namespace NCS.DSS.Customer.AzureSearchDataSyncTrigger
{
    public class CustomerSearchDataSyncTrigger
    {
        private readonly ILogger<CustomerSearchDataSyncTrigger> _logger;
        public CustomerSearchDataSyncTrigger(ILogger<CustomerSearchDataSyncTrigger> logger)
        {
            _logger = logger;
        }
        [Function("SyncDataForCustomerSearchTrigger")]
        public async Task RunAsync(
            [CosmosDBTrigger("customers", "customers", ConnectionStringSetting = "CustomerConnectionString",
                LeaseCollectionName = "customers-leases", CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<string> documents)
        {
            var functionName = nameof(CustomerSearchDataSyncTrigger);
            _logger.LogInformation("Function {functionName} has been invoked", functionName);
            if (documents.Count == 0)
            {
                _logger.LogInformation("No documents to process");
                return;
            }
            _logger.LogInformation("Initializing search service client");
            var client = SearchHelper.GetSearchServiceClient();
            _logger.LogInformation("Attempting to process {Count} document(s)", documents.Count);
            // Deserialize documents into strongly-typed CustomerSearch models
            var customers = documents.Select(doc =>
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<Models.CustomerSearch>(doc, options);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to deserialize document: {Document}", doc);
                    return null;
                }
            })
            .Where(customer => customer != null)
            .ToList();
            if (customers.Count == 0)
            {
                _logger.LogWarning("No valid documents to process after deserialization");
                return;
            }
            var batch = IndexDocumentsBatch.MergeOrUpload(customers);
            try
            {
                _logger.LogInformation("Merging or uploading document batch for indexing with {Count} document(s)", customers.Count);
                var results = await client.IndexDocumentsAsync(batch);
                var failed = results.Value.Results.Where(r => !r.Succeeded).Select(r => r.Key).ToList();
                if (failed.Count > 0)
                {
                    _logger.LogWarning("Failed to index some of the documents: {FailedDocuments}", string.Join(", ", failed));
                }
                _logger.LogInformation("Function {functionName} has finished processing successfully", functionName);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in {functionName}. Exception: {exMessage}", functionName, ex.Message);
                throw;
            }
        }
    }
}
