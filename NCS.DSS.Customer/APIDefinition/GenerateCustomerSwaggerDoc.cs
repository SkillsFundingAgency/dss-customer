﻿using DFC.Swagger.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Reflection;

namespace NCS.DSS.Customer.APIDefinition
{
    public class GenerateCustomerSwaggerDoc
    {
        public const string ApiTitle = "Customers";
        public const string ApiDefinitionName = "API-Definition";
        public const string ApiDefRoute = ApiTitle + "/" + ApiDefinitionName;
        public const string ApiDescription = "To support the Data Collections integration with DSS  PriorityGroups has been added as an attribute "
            + "and it supports multiple values in the form of a JSON array. With multiple groups we also now have new validation rules.";
        public const string ApiVersion = "4.0.0";
        private readonly ISwaggerDocumentGenerator _swaggerDocumentGenerator;

        public GenerateCustomerSwaggerDoc(ISwaggerDocumentGenerator swaggerDocumentGenerator)
        {
            _swaggerDocumentGenerator = swaggerDocumentGenerator;
        }

        [Function(ApiDefinitionName)]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ApiDefRoute)] HttpRequest req)
        {
            var swagger = _swaggerDocumentGenerator.GenerateSwaggerDocument(req, ApiTitle, ApiDescription,
                ApiDefinitionName, ApiVersion, Assembly.GetExecutingAssembly());

            if (string.IsNullOrEmpty(swagger))
                return new NoContentResult();

            return new OkObjectResult(swagger);
        }
    }
}
