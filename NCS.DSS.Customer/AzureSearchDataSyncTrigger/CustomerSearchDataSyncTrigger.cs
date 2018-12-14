using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.ReferenceData;
using Document = Microsoft.Azure.Documents.Document;

namespace NCS.DSS.Customer.AzureSearchDataSyncTrigger
{
    public static class CustomerSearchDataSyncTrigger
    {
        [FunctionName("SyncDataForCustomerSearchTrigger")]
        public static async Task Run(
            [CosmosDBTrigger("customers", "customers", ConnectionStringSetting = "CustomerConnectionString",
                LeaseCollectionName = "customers-leases", CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> documents,
            TraceWriter log)
        {
            log.Info("Entered SyncDataForCustomerSearchTrigger");

            SearchHelper.GetSearchServiceClient();

            log.Info("get search service client");

            var indexClient = SearchHelper.GetIndexClient();

            log.Info("get index client");
            
            log.Info("Documents modified " + documents.Count);

            if (documents.Count > 0)
            {
                var customers = documents.Select(doc => new Models.Customer()
                    {
                        CustomerId = doc.GetPropertyValue<Guid?>("id"),
                        DateOfRegistration = doc.GetPropertyValue<DateTime?>("DateOfRegistration"),
                        GivenName = doc.GetPropertyValue<string>("GivenName"),
                        FamilyName = doc.GetPropertyValue<string>("FamilyName"),
                        DateofBirth = doc.GetPropertyValue<DateTime?>("DateofBirth"),
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

                var batch = IndexBatch.MergeOrUpload(customers);
                
                try
                {
                    log.Info("attempting to merge docs to azure search");

                    await indexClient.Documents.IndexAsync(batch);

                    log.Info("successfully merged docs to azure search");

                }
                catch (IndexBatchException e)
                {
                    log.Error(string.Format("Failed to index some of the documents: {0}", 
                        string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key))));

                    log.Error(e.ToString());
                }
            }
        }
    }
}