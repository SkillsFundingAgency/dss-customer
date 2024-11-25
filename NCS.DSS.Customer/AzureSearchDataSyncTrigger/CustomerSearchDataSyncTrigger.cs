using Azure;
using Azure.Search.Documents.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.ReferenceData;
using Document = Microsoft.Azure.Documents.Document;

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
            IReadOnlyList<Document> documents)
        {
            var functionName = nameof(CustomerSearchDataSyncTrigger);
            _logger.LogInformation($"Function {functionName} has been invoked");

            _logger.LogInformation("Initializing search service client");
            var client = SearchHelper.GetSearchServiceClient();

            _logger.LogInformation($"Attempting to process {documents.Count} document(s)");

            if (documents.Count > 0)
            {
                var customers = documents.Select(doc => new Models.CustomerSearch()
                {
                    CustomerId = doc.GetPropertyValue<Guid?>("id"),
                    DateOfRegistration = doc.GetPropertyValue<DateTime?>("DateOfRegistration"),
                    Title = doc.GetPropertyValue<Title>("Title"),
                    GivenName = doc.GetPropertyValue<string>("GivenName"),
                    FamilyName = doc.GetPropertyValue<string>("FamilyName"),
                    DateofBirth = doc.GetPropertyValue<DateTime?>("DateofBirth"),
                    Gender = doc.GetPropertyValue<Gender?>("Gender"),
                    UniqueLearnerNumber = doc.GetPropertyValue<string>("UniqueLearnerNumber"),
                    OptInUserResearch = doc.GetPropertyValue<bool?>("OptInUserResearch"),
                    OptInMarketResearch = doc.GetPropertyValue<bool?>("OptInMarketResearch"),
                    DateOfTermination = doc.GetPropertyValue<DateTime?>("DateOfTermination"),
                    ReasonForTermination = doc.GetPropertyValue<ReasonForTermination?>("ReasonForTermination"),
                    IntroducedBy = doc.GetPropertyValue<IntroducedBy?>("IntroducedBy"),
                    IntroducedByAdditionalInfo = doc.GetPropertyValue<string>("IntroducedByAdditionalInfo"),
                    LastModifiedDate = doc.GetPropertyValue<DateTime?>("LastModifiedDate"),
                    LastModifiedTouchpointId = doc.GetPropertyValue<string>("LastModifiedTouchpointId")
                })
                    .ToList();

                var batch = IndexDocumentsBatch.MergeOrUpload(customers);


                try
                {
                    _logger.LogInformation($"Merging or uploading document batch for indexing with {documents.Count} document(s)");
                    var results = await client.IndexDocumentsAsync(batch);

                    var failed = results.Value.Results.Where(r => !r.Succeeded).Select(r => r.Key).ToList();

                    if (failed.Count > 0)
                    {
                        _logger.LogInformation(string.Format("Failed to index some of the documents: {0}", string.Join(", ", failed)));
                    }

                    _logger.LogInformation($"Function {functionName} has finished invoking");

                }
                catch (RequestFailedException ex)
                {
                    _logger.LogError(ex, $"An unexpected error occurred in {functionName}. Exception: {ex.Message}");
                    throw;
                }
            }
        }
    }
}