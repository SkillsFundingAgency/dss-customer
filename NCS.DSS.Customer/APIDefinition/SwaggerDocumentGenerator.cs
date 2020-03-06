using DFC.Functions.DI.Standard.Attributes;
using DFC.JSON.Standard.Attributes;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace DSS.Swagger.Standard
{
    public class SwaggerDocumentGenerator : ISwaggerDocumentGenerator
    {
        private bool IncludeSubcontractorId;
        private bool IncludeTouchpointId;

        public string GenerateSwaggerDocument(HttpRequest req, string apiTitle, string apiDescription, string apiDefinitionName, string apiVersion, Assembly assembly, bool includeSubcontractorId = true, bool includeTouchpointId = true, string pathPrefix = "/api/")
        {
            IncludeSubcontractorId = includeSubcontractorId;
            IncludeTouchpointId = includeTouchpointId;

            if (req == null)
                throw new ArgumentNullException(nameof(req));

            if (string.IsNullOrEmpty(apiTitle))
                throw new ArgumentNullException(nameof(apiTitle));

            if (string.IsNullOrEmpty(apiDescription))
                throw new ArgumentNullException(nameof(apiDescription));

            if (string.IsNullOrEmpty(apiDefinitionName))
                throw new ArgumentNullException(nameof(apiDefinitionName));

            if (string.IsNullOrEmpty(apiVersion))
                throw new ArgumentNullException(nameof(apiVersion));

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            dynamic doc = new ExpandoObject();
            doc.swagger = "2.0";
            doc.info = new ExpandoObject();
            doc.info.title = apiTitle;
            doc.info.version = apiVersion;
            doc.info.description = apiDescription;
            doc.host = req.Host.HasValue ? req.Host.Value : string.Empty;
            doc.basePath = "/";
            doc.schemes = new[] { "https" };
            if (req.Host.ToString().Contains("127.0.0.1") || req.Host.ToString().Contains("localhost"))
            {
                doc.schemes = new[] { "http" };
            }

            doc.definitions = new ExpandoObject();
            doc.paths = GeneratePaths(assembly, doc, apiTitle, apiDefinitionName, pathPrefix);
            doc.securityDefinitions = GenerateSecurityDefinitions();

            return JsonConvert.SerializeObject(doc);
        }

        private dynamic GenerateSecurityDefinitions()
        {
            dynamic securityDefinitions = new ExpandoObject();
            securityDefinitions.apikeyQuery = new ExpandoObject();
            securityDefinitions.apikeyQuery.type = "apiKey";
            securityDefinitions.apikeyQuery.name = "code";
            securityDefinitions.apikeyQuery.@in = "query";
            return securityDefinitions;
        }

        private dynamic GeneratePaths(Assembly assembly, dynamic doc, string apiTitle, string apiDefinitionName, string pathPrefix)
        {
            dynamic paths = new ExpandoObject();
            var methods = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(FunctionNameAttribute), false).Length > 0).ToArray();
            foreach (MethodInfo methodInfo in methods)
            {
                //hide any disabled methods
                if (methodInfo.GetCustomAttributes(typeof(DisableAttribute), true).Any() ||
                    methodInfo.GetCustomAttributes(typeof(SwaggerIgnoreAttribute), true).Any())
                    continue;

                var route = pathPrefix;

                var functionAttr = (FunctionNameAttribute)methodInfo.GetCustomAttributes(typeof(FunctionNameAttribute), false)
                    .Single();

                if (functionAttr.Name == apiDefinitionName) continue;

                HttpTriggerAttribute triggerAttribute = null;
                foreach (ParameterInfo parameter in methodInfo.GetParameters())
                {
                    triggerAttribute = parameter.GetCustomAttributes(typeof(HttpTriggerAttribute), false)
                        .FirstOrDefault() as HttpTriggerAttribute;
                    if (triggerAttribute != null) break;
                }
                if (triggerAttribute == null) continue; // Trigger attribute is required in an Azure function

                if (!string.IsNullOrWhiteSpace(triggerAttribute.Route))
                {
                    route += triggerAttribute.Route;
                }
                else
                {
                    route += functionAttr.Name;
                }

                dynamic path = new ExpandoObject();

                var verbs = triggerAttribute.Methods ?? new[] { "get", "post", "delete", "head", "patch", "put", "options" };
                foreach (string verb in verbs)
                {
                    dynamic operation = new ExpandoObject();
                    operation.operationId = ToTitleCase(functionAttr.Name);
                    operation.produces = new[] { "application/json" };
                    operation.consumes = new[] { "application/json" };
                    operation.parameters = GenerateFunctionParametersSignature(methodInfo, route, doc);

                    // Summary is title
                    operation.summary = GetFunctionName(methodInfo, functionAttr.Name);
                    // Verbose description
                    operation.description = GetFunctionDescription(methodInfo, functionAttr.Name);

                    operation.responses = GenerateResponseParameterSignature(methodInfo, doc);
                    operation.tags = new[] { apiTitle };

                    dynamic keyQuery = new ExpandoObject();
                    keyQuery.apikeyQuery = new string[0];
                    operation.security = new ExpandoObject[] { keyQuery };

                    AddToExpando(path, verb, operation);
                }
                AddToExpando(paths, route, path);
            }
            return paths;
        }

        private string GetFunctionDescription(MethodInfo methodInfo, string funcName)
        {
            var displayAttr = (DisplayAttribute)methodInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault();
            return !string.IsNullOrWhiteSpace(displayAttr?.Description) ? displayAttr.Description : $"This function will run {funcName}";
        }

        /// <summary>
        /// Max 80 characters in summary/title
        /// </summary>
        private string GetFunctionName(MethodInfo methodInfo, string funcName)
        {
            var displayAttr = (DisplayAttribute)methodInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault();
            if (!string.IsNullOrWhiteSpace(displayAttr?.Name))
            {
                return displayAttr.Name.Length > 80 ? displayAttr.Name.Substring(0, 80) : displayAttr.Name;
            }
            return $"Run {funcName}";
        }

        private string GetPropertyDescription(PropertyInfo propertyInfo)
        {
            var displayAttr = (DisplayAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault();

            return !string.IsNullOrWhiteSpace(displayAttr?.Description) ? displayAttr.Description : $"This returns {propertyInfo.PropertyType.Name}";
        }

        private dynamic GenerateResponseParameterSignature(MethodInfo methodInfo, dynamic doc)
        {
            dynamic responses = new ExpandoObject();
            dynamic responseDef = new ExpandoObject();

            var returnType = methodInfo.ReturnType;
            if (returnType.IsGenericType)
            {
                var genericReturnType = returnType.GetGenericArguments().FirstOrDefault();
                if (genericReturnType != null)
                {
                    returnType = genericReturnType;
                }
            }
            if (returnType == typeof(IActionResult) || returnType == typeof(HttpResponseMessage))
            {
                var responseTypeAttr = (ProducesResponseTypeAttribute)methodInfo
                    .GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false).FirstOrDefault();
                if (responseTypeAttr != null)
                {
                    returnType = responseTypeAttr.Type;
                }
                else
                {
                    returnType = typeof(void);
                }
            }
            if (returnType != typeof(void))
            {
                responseDef.schema = new ExpandoObject();

                if (returnType.Namespace == "System")
                {
                    SetParameterType(returnType, responseDef.schema, null);
                }
                else
                {
                    string name = returnType.Name;
                    if (returnType.IsGenericType)
                    {
                        var realType = returnType.GetGenericArguments()[0];
                        if (realType.Namespace == "System")
                        {
                            dynamic inlineSchema = GetObjectSchemaDefinition(null, returnType);
                            responseDef.schema = inlineSchema;
                        }
                        else
                        {
                            AddToExpando(responseDef.schema, "$ref", "#/definitions/" + name);
                            AddParameterDefinition((IDictionary<string, object>)doc.definitions, returnType);
                        }
                    }
                    else
                    {
                        AddToExpando(responseDef.schema, "$ref", "#/definitions/" + name);
                        AddParameterDefinition((IDictionary<string, object>)doc.definitions, returnType);
                    }
                }
            }

            // automatically get data(http code, description and show schema) from the new custom response class
            var responseCodes = methodInfo.GetCustomAttributes(typeof(Response), false);

            foreach (var response in responseCodes)
            {
                var customerResponse = (Response)response;

                if (!customerResponse.ShowSchema)
                    responseDef = new ExpandoObject();

                responseDef.description = customerResponse.Description;
                AddToExpando(responses, customerResponse.HttpStatusCode.ToString(), responseDef);
            }

            return responses;
        }

        private List<object> GenerateFunctionParametersSignature(MethodInfo methodInfo, string route, dynamic doc)
        {
            var parameterSignatures = new List<object>();

            if (IncludeTouchpointId)
            {
                dynamic opHeaderParam = new ExpandoObject();
                opHeaderParam.name = "TouchpointId";
                opHeaderParam.@in = "header";
                opHeaderParam.required = true;
                opHeaderParam.type = "string";
                parameterSignatures.Add(opHeaderParam);
            }

            if (IncludeSubcontractorId)
            {
                dynamic opHeaderParam2 = new ExpandoObject();
                opHeaderParam2.name = "SubcontractorId";
                opHeaderParam2.@in = "header";
                opHeaderParam2.required = false;
                opHeaderParam2.type = "string";
                parameterSignatures.Add(opHeaderParam2);
            }

            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                if (parameter.ParameterType == typeof(HttpRequest)) continue;
                if (parameter.ParameterType == typeof(Microsoft.Extensions.Logging.ILogger)) continue;
                if (parameter.GetCustomAttributes().Any(attr => attr is InjectAttribute)) continue;

                bool hasUriAttribute = parameter.GetCustomAttributes().Any(attr => attr is FromQueryAttribute);

                if (route.Contains('{' + parameter.Name))
                {
                    dynamic opParam = new ExpandoObject();
                    opParam.name = parameter.Name;
                    opParam.@in = "path";
                    opParam.required = true;
                    SetParameterType(parameter.ParameterType, opParam, null);
                    parameterSignatures.Add(opParam);
                }
                else if (hasUriAttribute && parameter.ParameterType.Namespace == "System")
                {
                    dynamic opParam = new ExpandoObject();
                    opParam.name = parameter.Name;
                    opParam.@in = "query";
                    opParam.required = parameter.GetCustomAttributes().Any(attr => attr is RequiredAttribute);
                    SetParameterType(parameter.ParameterType, opParam, doc.definitions);
                    parameterSignatures.Add(opParam);
                }
                else if (hasUriAttribute && parameter.ParameterType.Namespace != "System")
                {
                    AddObjectProperties(parameter.ParameterType, "", parameterSignatures, doc);
                }
                else
                {
                    dynamic opParam = new ExpandoObject();
                    opParam.name = parameter.Name;
                    opParam.@in = "body";
                    opParam.required = true;
                    opParam.schema = new ExpandoObject();
                    if (parameter.ParameterType.Namespace == "System")
                    {
                        SetParameterType(parameter.ParameterType, opParam.schema, null);
                    }
                    else
                    {
                        AddToExpando(opParam.schema, "$ref", "#/definitions/" + parameter.ParameterType.Name);
                        AddParameterDefinition((IDictionary<string, object>)doc.definitions, parameter.ParameterType);
                    }
                    parameterSignatures.Add(opParam);
                }
            }
            return parameterSignatures;
        }

        private void AddObjectProperties(Type t, string parentName, List<object> parameterSignatures, dynamic doc)
        {
            var publicProperties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in publicProperties)
            {
                if (!string.IsNullOrWhiteSpace(parentName))
                {
                    parentName += ".";
                }
                if (property.PropertyType.Namespace != "System")
                {
                    AddObjectProperties(property.PropertyType, parentName + property.Name, parameterSignatures, doc);
                }
                else
                {
                    dynamic opParam = new ExpandoObject();

                    opParam.name = parentName + property.Name;
                    opParam.@in = "query";
                    opParam.required = property.GetCustomAttributes().Any(attr => attr is RequiredAttribute);
                    opParam.description = GetPropertyDescription(property);
                    SetParameterType(property.PropertyType, opParam, doc.definitions);
                    parameterSignatures.Add(opParam);
                }
            }
        }

        private void AddParameterDefinition(IDictionary<string, object> definitions, Type parameterType)
        {
            if (!definitions.TryGetValue(parameterType.Name, out dynamic objDef))
            {
                objDef = GetObjectSchemaDefinition(definitions, parameterType);
                definitions.Add(parameterType.Name, objDef);
            }
        }

        private dynamic GetObjectSchemaDefinition(IDictionary<string, object> definitions, Type parameterType)
        {
            dynamic objDef = new ExpandoObject();
            objDef.type = "object";
            objDef.properties = new ExpandoObject();
            var publicProperties = parameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<string> requiredProperties = new List<string>();
            foreach (PropertyInfo property in publicProperties)
            {
                if (property.GetCustomAttributes().Any(attr => attr is JsonIgnoreOnSerialize))
                    continue;

                if (property.GetCustomAttributes().Any(attr => attr is RequiredAttribute))
                {
                    requiredProperties.Add(property.Name);
                }

                dynamic propDef = new ExpandoObject();
                propDef.description = GetPropertyDescription(property);

                var stringAttribute = (StringLengthAttribute)property.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault();
                if (stringAttribute != null)
                {
                    propDef.maxLength = stringAttribute.MaximumLength;
                    propDef.minLength = stringAttribute.MinimumLength;
                }

                var regexAttribute = (RegularExpressionAttribute)property.GetCustomAttributes(typeof(RegularExpressionAttribute), false).FirstOrDefault();
                if (regexAttribute != null)
                    propDef.pattern = regexAttribute.Pattern;

                var exampleAttribute = (Example)property.GetCustomAttributes(typeof(Example), false).FirstOrDefault();

                SetParameterType(property.PropertyType, propDef, definitions, exampleAttribute?.Description);
                AddToExpando(objDef.properties, property.Name, propDef);
            }
            if (requiredProperties.Count > 0)
            {
                objDef.required = requiredProperties;
            }
            return objDef;
        }

        private void SetParameterType(Type parameterType, dynamic opParam, dynamic definitions, string exampleDescription = "")
        {
            var inputType = parameterType;
            string paramType = parameterType.UnderlyingSystemType.ToString();

            var isEnum = parameterType.IsEnum;
            var isNullableEnum = Nullable.GetUnderlyingType(parameterType)?.IsEnum == true;

            var setObject = opParam;
            if ((inputType.IsArray || inputType.GetInterface(typeof(System.Collections.IEnumerable).Name, false) != null) && inputType != typeof(String))
            {
                
                opParam.type = "array";
                if (!string.IsNullOrWhiteSpace(exampleDescription)) setObject.example = exampleDescription;
                opParam.items = new ExpandoObject();
                setObject = opParam.items;

                if (inputType.IsArray)
                {
                    parameterType = parameterType.GetElementType();
                }
                else if (inputType.IsGenericType && inputType.GenericTypeArguments.Length == 1)
                {
                    parameterType = inputType.GetGenericArguments()[0];;
 
                    if (parameterType.IsEnum)
                    {
                        var enumValues = new List<string>();
                        foreach (var item in Enum.GetValues(parameterType))
                        {
                            var enumName = item.ToString();

                            if (enumName != null)
                            {
                                var memInfo = parameterType.GetMember(enumName);
                                var descriptionAttributes =
                                    memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                                var description = string.Empty;

                                if (descriptionAttributes.Length > 0)
                                    description = ((DescriptionAttribute)descriptionAttributes[0]).Description;

                                if (string.IsNullOrEmpty(description))
                                    description = item.ToString();

                                enumValues.Add(Convert.ToInt32(item) + " - " + description);
                            }
                        }

                        if (enumValues.Any())
                            opParam.items.@enum = enumValues.ToArray();
                    }
                }
            }

            if (inputType.Namespace == "System" && !isNullableEnum && !isEnum || (inputType.IsGenericType && inputType.GetGenericArguments()[0].Namespace == "System"))
            {
                if (paramType.Contains("System.DateTime"))
                {
                    setObject.format = "date-time";
                    setObject.type = "string";
                    if (!string.IsNullOrWhiteSpace(exampleDescription)) setObject.example = DateTime.Parse(exampleDescription);
                }
                else if (paramType.Contains("System.Int32"))
                {
                    setObject.format = "int32";
                    setObject.type = "integer";
                    if (!string.IsNullOrWhiteSpace(exampleDescription)) setObject.example = int.Parse(exampleDescription);
                }
                else if (paramType.Contains("System.Int64"))
                {
                    setObject.format = "int64";
                    setObject.type = "integer";
                    if (!string.IsNullOrWhiteSpace(exampleDescription)) setObject.example = int.Parse(exampleDescription);
                }
                else if (paramType.Contains("System.Single"))
                {
                    setObject.format = "float";
                    setObject.type = "number";
                    if (!string.IsNullOrWhiteSpace(exampleDescription)) setObject.example = float.Parse(exampleDescription);
                }
                else if (paramType.Contains("System.Decimal"))
                {
                    setObject.format = "decimal";
                    setObject.type = "number";
                    if (!string.IsNullOrWhiteSpace(exampleDescription)) setObject.example = decimal.Parse(exampleDescription);
                }
                else if (paramType.Contains("System.Double"))
                {
                    setObject.format = "double";
                    setObject.type = "number";
                    if (!string.IsNullOrWhiteSpace(exampleDescription)) setObject.example = double.Parse(exampleDescription);
                }
                else if (paramType.Contains("System.Boolean"))
                {
                    setObject.type = "boolean";
                    if (!string.IsNullOrWhiteSpace(exampleDescription)) setObject.example = bool.Parse(exampleDescription);
                }
                else
                {
                    setObject.type = "string";
                    if (!string.IsNullOrWhiteSpace(exampleDescription)) setObject.example = exampleDescription;
                }
            }
            else if (isEnum || isNullableEnum)
            {
                opParam.type = "string";
                var enumValues = new List<string>();

                if (isEnum)
                {
                    foreach (var item in Enum.GetValues(inputType))
                    {
                        var enumName = inputType.GetEnumName(item);

                        if (enumName != null)
                        {
                            var memInfo = inputType.GetMember(enumName);
                            var descriptionAttributes =
                                memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                            var description = string.Empty;

                            if (descriptionAttributes.Length > 0)
                                description = ((DescriptionAttribute)descriptionAttributes[0]).Description;

                            if (string.IsNullOrEmpty(description))
                                description = item.ToString();

                            enumValues.Add(Convert.ToInt32(item) + " - " + description);
                        }
                    }
                }

                if (isNullableEnum)
                {
                    var enumType = Nullable.GetUnderlyingType(inputType);

                    if (enumType != null)
                    {
                        foreach (var item in Enum.GetValues(enumType))
                        {
                            var memInfo = Nullable.GetUnderlyingType(inputType)?.GetMember(item.ToString());
                            var descriptionAttributes =
                                memInfo?[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                            var description = string.Empty;

                            if (descriptionAttributes?.Length > 0)
                                description = ((DescriptionAttribute)descriptionAttributes[0]).Description;

                            if (string.IsNullOrEmpty(description))
                                description = item.ToString();

                            enumValues.Add(Convert.ToInt32(item) + " - " + description);
                        }
                    }
                }

                if (enumValues.Any())
                    opParam.@enum = enumValues.ToArray();
            }
            else if (definitions != null)
            {
                AddToExpando(setObject, "$ref", "#/definitions/" + parameterType.Name);
                AddParameterDefinition((IDictionary<string, object>)definitions, parameterType);
            }
        }

        private string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }

        private void AddToExpando(ExpandoObject obj, string name, object value)
        {
            if (((IDictionary<string, object>)obj).ContainsKey(name))
            {
                // Fix for functions with same routes but different verbs
                var existing = (IDictionary<string, object>)((IDictionary<string, object>)obj)[name];
                var append = (IDictionary<string, object>)value;
                foreach (KeyValuePair<string, object> keyValuePair in append)
                {
                    existing.Add(keyValuePair);
                }
            }
            else
            {
                ((IDictionary<string, object>)obj).Add(name, value);
            }
        }
    }
}