using Azure;
using Azure.Search.Documents.Models;
using DFC.Common.Standard.Logging;
using Microsoft.Azure.Functions.Worker;
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
        private readonly ILogger<CustomerSearchDataSyncTrigger> _log;
        public CustomerSearchDataSyncTrigger(ILoggerHelper loggerHelper, ILogger<CustomerSearchDataSyncTrigger> log)
        {
            _loggerHelper = loggerHelper;
            _log = log;
        }

        [Function("SyncDataForCustomerSearchTrigger")]
        public async Task Run(
            [CosmosDBTrigger("customers", "customers", ConnectionStringSetting = "CustomerConnectionString",
                LeaseCollectionName = "customers-leases", CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> documents)
        {
            var correlationId = Guid.NewGuid();

            _loggerHelper.LogMethodEnter(_log);

            var client = SearchHelper.GetSearchServiceClient();

            _loggerHelper.LogInformationMessage(_log, correlationId, "get search service client");


            _loggerHelper.LogInformationMessage(_log, correlationId, "get index client");

            _loggerHelper.LogInformationMessage(_log, correlationId, "Documents modified " + documents.Count);

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
                    _log.LogInformation("attempting to merge docs to azure search");

                    var results = await client.IndexDocumentsAsync(batch);

                    var failed = results.Value.Results.Where(r => !r.Succeeded).Select(r => r.Key).ToList();

                    if (failed.Count > 0)
                    {
                        _loggerHelper.LogInformationMessage(_log, correlationId, string.Format("Failed to index some of the documents: {0}", string.Join(", ", failed)));
                    }

                    _log.LogInformation("successfully merged docs to azure search");

                }
                catch (RequestFailedException e)
                {
                    _loggerHelper.LogException(_log, correlationId, e);
                    
                }
            }
        }
    }
}