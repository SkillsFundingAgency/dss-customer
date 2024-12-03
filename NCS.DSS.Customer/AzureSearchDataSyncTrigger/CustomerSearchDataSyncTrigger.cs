using Azure;
using Azure.Search.Documents.Models;
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
                    //var customers = new List<Models.CustomerSearch>();
                    //foreach (var doc in documents)
                    //{                        
                    //    var root = doc.RootElement;
                    //    try
                    //    {
                    //        _logger.LogInformation("Retrieving data from customer with id {id}", root.GetProperty("id").GetString());
                    //        var cust = new Models.CustomerSearch()
                    //        {
                    //            CustomerId = root.GetProperty("id").GetGuid(),
                    //            DateOfRegistration = root.GetProperty("DateOfRegistration").GetDateTime(),
                    //            GivenName = root.GetProperty("GivenName").GetString(),
                    //            FamilyName = root.GetProperty("FamilyName").GetString(),
                    //            UniqueLearnerNumber = root.GetProperty("UniqueLearnerNumber").GetString(),
                    //            OptInUserResearch = root.GetProperty("OptInUserResearch").GetBoolean(),
                    //            OptInMarketResearch = root.GetProperty("OptInMarketResearch").GetBoolean(),
                    //            IntroducedByAdditionalInfo = root.GetProperty("IntroducedByAdditionalInfo").GetString(),
                    //            LastModifiedTouchpointId = root.GetProperty("LastModifiedTouchpointId").GetString()
                    //        };

                    //        var title = root.GetProperty("Title").GetInt32();
                    //        if (Enum.IsDefined(typeof(Title), title))
                    //            cust.Title =(Title) title;

                    //        cust.DateofBirth = root.GetProperty("DateofBirth").GetDateTime();

                    //        var gen = root.GetProperty("Gender").GetInt32();
                    //        if (Enum.IsDefined(typeof(Gender), gen))
                    //            cust.Gender = (Gender) gen;

                    //       cust.DateOfTermination = root.GetProperty("DateOfTermination").GetDateTime();

                    //        var rot = root.GetProperty("ReasonForTermination").GetInt32();
                    //        if (Enum.IsDefined(typeof(ReasonForTermination), gen))
                    //            cust.ReasonForTermination =(ReasonForTermination) rot;

                    //        var intro = root.GetProperty("IntroducedBy").GetInt32();
                    //        if (Enum.IsDefined(typeof(IntroducedBy), intro))
                    //            cust.IntroducedBy = (IntroducedBy) intro;
                            
                    //        cust.LastModifiedDate = root.GetProperty("LastModifiedDate").GetDateTime();

                    //        customers.Add(cust);

                    //        _logger.LogInformation("Completed retrieving data from customer with id {id}", root.GetProperty("id").GetString());
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        _logger.LogError("Failed to retrieve data from customer with id {id} {error}", root.GetProperty("id").GetString(),ex.StackTrace);
                    //    }                        
                    //}

                    var batch = IndexDocumentsBatch.MergeOrUpload(documents);

                    _logger.LogInformation("attempting to merge docs to azure search");

                    _logger.LogInformation("Document IDs : {Ids}",string.Join(',', documents.Select(d => d.CustomerId).ToArray()));

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
                    _logger.LogError("Request failed Excpetion with {error} {stacktrace}", e.Message, e.StackTrace);
                    throw;
                }               
            } 
            _logger.LogInformation("{functionName} existed", nameof(CustomerSearchDataSyncTrigger));
        }
    }
}