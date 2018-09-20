using System;
using System.Configuration;
using Microsoft.Azure.Documents.Client;

namespace NCS.DSS.Customer.Cosmos.Helper
{
    public static class DocumentDBHelper
    {
        private static Uri _documentCollectionUri;
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["DatabaseId"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["CollectionId"];

        private static Uri _subscriptionDocumentCollectionUri;
        private static readonly string SubscriptionDatabaseId = ConfigurationManager.AppSettings["SubscriptionDatabaseId"];
        private static readonly string SubscriptionCollectionId = ConfigurationManager.AppSettings["SubscriptionCollectionId"];

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
        
    }
}
