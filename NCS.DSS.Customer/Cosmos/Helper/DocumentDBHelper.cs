using System;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using NCS.DSS.Customer.Cosmos.Helper;

namespace NCS.DSS.Customer.Cosmos.Helper
{
    public class DocumentDBHelper : IDocumentDBHelper
    {
        private Uri _documentCollectionUri;
        private Uri _documentUri;
        private readonly string _databaseId = ConfigurationManager.AppSettings["DatabaseId"];
        private readonly string _collectionId = ConfigurationManager.AppSettings["CollectionId"];

        private Uri _subscriptionDocumentCollectionUri;
        private readonly string _subscriptionDatabaseId = ConfigurationManager.AppSettings["SubscriptionDatabaseId"];
        private readonly string _subscriptionCollectionId = ConfigurationManager.AppSettings["SubscriptionCollectionId"];

        public Uri CreateDocumentCollectionUri()
        {
            if (_documentCollectionUri != null)
                return _documentCollectionUri;

            _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(
                _databaseId,
                _collectionId);

            return _documentCollectionUri;
        }


        public Uri CreateDocumentUri(Guid? customerId)
        {
            if (_documentUri != null)
                return _documentUri;

            _documentUri = UriFactory.CreateDocumentUri(_databaseId, _collectionId, customerId.ToString());

            return _documentUri;

        }


        #region SubscriptionDB

        public Uri CreateSubscriptionDocumentCollectionUri()
        {
            if (_subscriptionDocumentCollectionUri != null)
                return _subscriptionDocumentCollectionUri;

            _subscriptionDocumentCollectionUri = UriFactory.CreateDocumentCollectionUri(
                _subscriptionDatabaseId, _subscriptionCollectionId);

            return _subscriptionDocumentCollectionUri;
        }

        #endregion 


    }
}
