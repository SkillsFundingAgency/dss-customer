using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace DSS.Swagger.Standard
{
    public interface ISwaggerDocumentGenerator
    {
        string GenerateSwaggerDocument(HttpRequest req, string apiTitle, string apiDescription,
            string apiDefinitionName, string apiVersion, Assembly assembly, bool includeSubcontractorId = true, bool includeTouchpointId = true, string pathPrefix = "/api/");
    }
}