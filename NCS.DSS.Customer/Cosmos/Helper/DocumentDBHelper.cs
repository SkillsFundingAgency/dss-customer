using System;
using Microsoft.Azure.Documents.Client;

namespace NCS.DSS.Customer.Cosmos.Helper
{
    public static class DocumentDBHelper
    {
        private static Uri _documentCollectionUri;
        private static readonly string DatabaseId = Environment.GetEnvironmentVariable("DatabaseId");
        private static readonly string CollectionId = Environment.GetEnvironmentVariable("CollectionId");

        private static Uri _subscriptionDocumentCollectionUri;
        private static readonly string SubscriptionDatabaseId = Environment.GetEnvironmentVariable("SubscriptionDatabaseId");
        private static readonly string SubscriptionCollectionId = Environment.GetEnvironmentVariable("SubscriptionCollectionId");
        private static readonly string DigitalIdentityDatabaseId = Environment.GetEnvironmentVariable("DigitalIdentityDatabaseId");
        private static readonly string DigitalIdentityCollectionId = Environment.GetEnvironmentVariable("DigitalIdentityCollectionId");

        public static Uri CreateDocumentCollectionUri()
        {
            if (_documentCollectionUri != null)
                return _documentCollectionUri;

            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(
                DatabaseId,
                CollectionId);

            return _documentCollectionUri;
        }
        
        public static Uri CreateDocumentUri(Guid? customerId)
        {
            return UriFactory.CreateDocumentUri(DatabaseId, CollectionId, customerId.ToString());
        }
        
        #region SubscriptionDB

        public static Uri CreateSubscriptionDocumentCollectionUri()
        {
            if (_subscriptionDocumentCollectionUri != null)
                return _subscriptionDocumentCollectionUri;

            _subscriptionDocumentCollectionUri = UriFactory.CreateDocumentCollectionUri(
                SubscriptionDatabaseId, SubscriptionCollectionId);

            return _subscriptionDocumentCollectionUri;
        }
        #endregion

        public static Uri CreateDigitalIdentityDocumentUri()
        {
            return UriFactory.CreateDocumentCollectionUri(DigitalIdentityDatabaseId, DigitalIdentityCollectionId);
        }

        public static Uri CreateDigitalIdentityDocumentUri(Guid identityId)
        {
            return UriFactory.CreateDocumentUri(DigitalIdentityDatabaseId, DigitalIdentityCollectionId, identityId.ToString());
        }

    }
}
