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
                        var root = doc.RootElement;
                        var cust = new Models.CustomerSearch()
                        {
                            CustomerId = root.GetProperty("id").GetGuid(),                                
                            DateOfRegistration = root.GetProperty("DateOfRegistration").GetDateTime(),                            
                            GivenName = root.GetProperty("GivenName").GetString(),
                            FamilyName = root.GetProperty("FamilyName").GetString(),
                            UniqueLearnerNumber = root.GetProperty("UniqueLearnerNumber").GetString(),
                            OptInUserResearch = root.GetProperty("OptInUserResearch").GetBoolean(),
                            OptInMarketResearch = root.GetProperty("OptInMarketResearch").GetBoolean(),
                            IntroducedByAdditionalInfo = root.GetProperty("IntroducedByAdditionalInfo").GetString(),
                            LastModifiedTouchpointId = root.GetProperty("LastModifiedTouchpointId").GetString()
                        };

                        var title = Title.NotProvided;
                        if(Enum.TryParse(root.GetProperty("Title").GetString(), out title))
                            cust.Title = title;

                        var dob = DateTime.Now;
                        if (Enum.TryParse(root.GetProperty("DateofBirth").GetString(), out dob))
                            cust.DateofBirth = dob;

                        var gen = Gender.NotProvided;
                        if (Enum.TryParse(root.GetProperty("Gender").GetString(), out gen))
                            cust.Gender = gen;

                        var dot = DateTime.Now;
                        if (Enum.TryParse(root.GetProperty("DateOfTermination").GetString(), out dot))
                            cust.DateOfTermination = dot;

                        var rot = ReasonForTermination.CustomerChoice;
                        if (Enum.TryParse(root.GetProperty("ReasonForTermination").GetString(), out rot))
                            cust.ReasonForTermination = rot;

                        var intro = IntroducedBy.NotProvided;
                        if (Enum.TryParse(root.GetProperty("IntroducedBy").GetString(), out intro))
                            cust.IntroducedBy = intro;

                        var lmd = DateTime.Now;
                        if (Enum.TryParse(root.GetProperty("LastModifiedDate").GetString(), out lmd))
                            cust.LastModifiedDate = lmd;

                        customers.Add(cust);                        
                    }

                    var batch = IndexDocumentsBatch.MergeOrUpload(customers);

                    _logger.LogInformation("attempting to merge docs to azure search");

                    var results = await client.IndexDocumentsAsync(batch);

                    var failed = results.Value.Results.Where(r => !r.Succeeded).Select(r => r.Key).ToList();

                    if (failed.Count > 0)
                    {
                        _logger.LogInformation("Failed to index some of the documents: {errors}", string.Join(", ", failed));
                    }

                    _logger.LogInformation("successfully merged docs to azure search");

                }
                catch (Exception e)
                {
                    _logger.LogError("Request failed Excpetion with {error}", e.Message);

                }               
            } 
            _logger.LogInformation("{functionName} existed", nameof(CustomerSearchDataSyncTrigger));
        }
    }
}