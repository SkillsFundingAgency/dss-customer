﻿using Azure;
using Azure.Search.Documents;


namespace NCS.DSS.Customer.Helpers
{
    public static class SearchHelper
    {
        // https://learn.microsoft.com/en-us/azure/search/search-dotnet-sdk-migration-version-11

        private static readonly string SearchServiceName = Environment.GetEnvironmentVariable("SearchServiceName");
        private static readonly string SearchServiceKey = Environment.GetEnvironmentVariable("SearchServiceAdminApiKey");
        private static readonly string SearchServiceIndexName = Environment.GetEnvironmentVariable("CustomerSearchIndexName");

        private static SearchClient _client;

        public static SearchClient GetSearchServiceClient()
        {
            if (_client != null)
                return _client;

            _client = new SearchClient(new Uri($"https://{SearchServiceName}.search.windows.net"), SearchServiceIndexName, new AzureKeyCredential(SearchServiceKey));

            return _client;
        }

    }
}