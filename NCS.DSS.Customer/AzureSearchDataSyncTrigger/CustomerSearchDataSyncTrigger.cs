using Azure;
using Azure.Search.Documents.Models;
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
            IReadOnlyList<JsonDocument> documents)
        {
            var correlationId = Guid.NewGuid();

            _logger.LogInformation("{functionName} started",nameof(CustomerSearchDataSyncTrigger));

            var client = SearchHelper.GetSearchServiceClient();

            _logger.LogInformation("{correlationId} get search service client",correlationId);


            _logger.LogInformation("{correlationId} get index client", correlationId);

            _logger.LogInformation("{correlationId} Documents modified {count}", correlationId,documents.Count);

            if (documents.Count > 0)
            {
                var customers = documents.Select(doc => new Models.CustomerSearch()
                {
                    CustomerId = doc.RootElement.GetProperty("id").GetGuid(),
                    DateOfRegistration = doc.RootElement.GetProperty("DateOfRegistration").GetDateTime(),
                    Title =  Enum.Parse<Title>(doc.RootElement.GetProperty("Title").GetString()),
                    GivenName = doc.RootElement.GetProperty("GivenName").GetString(),
                    FamilyName = doc.RootElement.GetProperty("FamilyName").GetString(),
                    DateofBirth = Enum.Parse<DateTime>(doc.RootElement.GetProperty("DateofBirth").GetString()),
                    Gender = Enum.Parse<Gender>(doc.RootElement.GetProperty("Gender").GetString()),
                    UniqueLearnerNumber = doc.RootElement.GetProperty("UniqueLearnerNumber").GetString(),
                    OptInUserResearch = doc.RootElement.GetProperty("OptInUserResearch").GetBoolean(),
                    OptInMarketResearch = doc.RootElement.GetProperty("OptInMarketResearch").GetBoolean(),
                    DateOfTermination = Enum.Parse<DateTime>(doc.RootElement.GetProperty("DateOfTermination").GetString()),
                    ReasonForTermination = Enum.Parse<ReasonForTermination>(doc.RootElement.GetProperty("ReasonForTermination").GetString()),
                    IntroducedBy = Enum.Parse<IntroducedBy>(doc.RootElement.GetProperty("IntroducedBy").GetString()),
                    IntroducedByAdditionalInfo = doc.RootElement.GetProperty("IntroducedByAdditionalInfo").GetString(),
                    LastModifiedDate = Enum.Parse<DateTime>(doc.RootElement.GetProperty("LastModifiedDate").GetString()),
                    LastModifiedTouchpointId = doc.RootElement.GetProperty("LastModifiedTouchpointId").GetString()
                })
                    .ToList();

                var batch = IndexDocumentsBatch.MergeOrUpload(customers);


                try
                {
                    _logger.LogInformation("attempting to merge docs to azure search");

                    var results = await client.IndexDocumentsAsync(batch);

                    var failed = results.Value.Results.Where(r => !r.Succeeded).Select(r => r.Key).ToList();

                    if (failed.Count > 0)
                    {
                        _logger.LogInformation("{correlationId} Failed to index some of the documents: {errors}", correlationId, string.Join(", ", failed));
                    }

                    _logger.LogInformation("successfully merged docs to azure search");

                }
                catch (RequestFailedException e)
                {
                    _logger.LogError("{correlationId} Request failed Excpetion with {error}",correlationId, e.Message);

                }
                _logger.LogInformation("{functionName} existed", nameof(CustomerSearchDataSyncTrigger));
            }
        }
    }
}