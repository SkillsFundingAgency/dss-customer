using Azure;
using Azure.Search.Documents.Models;
using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
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

            var client = SearchHelper.GetSearchServiceClient();

            _loggerHelper.LogInformationMessage(log, _correlationId, "get search service client");


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

                var batch = IndexDocumentsBatch.MergeOrUpload(customers);


                try
                {
                    log.LogInformation("attempting to merge docs to azure search");

                    var results = await client.IndexDocumentsAsync(batch);

                    var failed = results.Value.Results.Where(r => !r.Succeeded).Select(r => r.Key).ToList();

                    if (failed.Count > 0)
                    {
                        _loggerHelper.LogInformationMessage(log, _correlationId, string.Format("Failed to index some of the documents: {0}", string.Join(", ", failed)));
                    }

                    log.LogInformation("successfully merged docs to azure search");

                }
                catch (RequestFailedException e)
                {
                    _loggerHelper.LogException(log, _correlationId, e);
                    
                }
            }
        }
    }
}