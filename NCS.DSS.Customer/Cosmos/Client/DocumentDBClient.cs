using Microsoft.Azure.Documents.Client;
using System;

namespace NCS.DSS.Customer.Cosmos.Client
{
    public static class DocumentDBClient
    {
        private static DocumentClient _documentClient;

        public static DocumentClient CreateDocumentClient()
        {
            if (_documentClient != null)
                return _documentClient;

            _documentClient = InitialiseDocumentClient();

            return _documentClient;
        }

        private static DocumentClient InitialiseDocumentClient()
        {
            string connectionString;

            try
            {
                connectionString = Environment.GetEnvironmentVariable("CustomerConnectionString");
            }
            catch (Exception e)
            {
                throw;
            }

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException();

            string endPoint;
            try
            {
                endPoint = connectionString.Split(new[] { "AccountEndpoint=" }, StringSplitOptions.None)[1]
                    .Split(';')[0]
                    .Trim();
            }
            catch (Exception e)
            {
                throw;
            }

            if (string.IsNullOrWhiteSpace(endPoint))
                throw new ArgumentNullException();

            string key;
            try
            {
                key = connectionString.Split(new[] { "AccountKey=" }, StringSplitOptions.None)[1]
                    .Split(';')[0]
                    .Trim();
            }
            catch (Exception e)
            {
                throw;
            }

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException();

            return new DocumentClient(new Uri(endPoint), key);
        }
    }
}
