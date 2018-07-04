using Microsoft.Azure.Documents.Client;

namespace NCS.DSS.Customer.Cosmos.Client
{
    public interface IDocumentDBClient
    {
        DocumentClient CreateDocumentClient();
    }
}