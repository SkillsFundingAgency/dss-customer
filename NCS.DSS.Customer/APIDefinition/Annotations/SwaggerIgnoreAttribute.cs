using System;

namespace DSS.Swagger.Standard.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class SwaggerIgnoreAttribute : Attribute
    {
    }
}