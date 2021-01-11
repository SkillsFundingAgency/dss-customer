using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Document = Microsoft.Azure.Documents.Document;

namespace NCS.DSS.Customer.AzureSearchDataSyncTrigger
{
    public class CustomerSearchDataSyncTrigger
    {
        private readonly ILoggerHelper _loggerHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        public CustomerSearchDataSyncTrigger(ILoggerHelper loggerHelper, IHttpRequestHelper httpRequestHelper)
        {
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
        }

        [FunctionName("SyncDataForCustomerSearchTrigger")]
        public async Task Run(
            [CosmosDBTrigger("customers", "customers", ConnectionStringSetting = "CustomerConnectionString",
                LeaseCollectionName = "customers-leases", CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> documents,
            ILogger log)
        {
            var _correlationId = Guid.NewGuid();

            _loggerHelper.LogMethodEnter(log);

            SearchHelper.GetSearchServiceClient();

            _loggerHelper.LogInformationMessage(log, _correlationId, "get search service client");

            var indexClient = SearchHelper.GetIndexClient();

            _loggerHelper.LogInformationMessage(log, _correlationId, "get index client");

            _loggerHelper.LogInformationMessage(log, _correlationId, "Documents modified " + documents.Count);

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

                var batch = IndexBatch.MergeOrUpload(customers);

                try
                {
                    log.LogInformation("attempting to merge docs to azure search");

                    await indexClient.Documents.IndexAsync(batch);

                    log.LogInformation("successfully merged docs to azure search");

                }
                catch (IndexBatchException e)
                {
                    _loggerHelper.LogInformationMessage(log, _correlationId, string.Format("Failed to index some of the documents: {0}",
                        string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key))));

                    _loggerHelper.LogException(log, _correlationId, e);
                }
            }
        }
    }
}