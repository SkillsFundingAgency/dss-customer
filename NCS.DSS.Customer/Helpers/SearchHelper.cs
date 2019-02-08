using System;
using System.Configuration;
using Microsoft.Azure.Search;

namespace NCS.DSS.Customer.Helpers
{
    public static class SearchHelper
    {        
        private static readonly string SearchServiceName = Environment.GetEnvironmentVariable("SearchServiceName");
        private static readonly string SearchServiceKey = Environment.GetEnvironmentVariable("SearchServiceAdminApiKey");
        private static readonly string SearchServiceIndexName = Environment.GetEnvironmentVariable("CustomerSearchIndexName");

        private static SearchServiceClient _serviceClient;
        private static ISearchIndexClient _indexClient;

        public static SearchServiceClient GetSearchServiceClient()
        {
            if (_serviceClient != null)
                return _serviceClient;

            _serviceClient = new SearchServiceClient(SearchServiceName, new SearchCredentials(SearchServiceKey));

            return _serviceClient;
        }
        
        public static ISearchIndexClient GetIndexClient()
        {
            if (_indexClient != null)
                return _indexClient;

            _indexClient = _serviceClient?.Indexes?.GetClient(SearchServiceIndexName);

            return _indexClient;
        }

    }
}