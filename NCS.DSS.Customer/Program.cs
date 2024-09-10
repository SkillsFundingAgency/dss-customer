using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.ServiceBus;
using NCS.DSS.Customer.Validation;
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddLogging();
        services.AddSingleton<IResourceHelper, ResourceHelper>();
        services.AddSingleton<IValidate, Validate>();
        services.AddSingleton<ILoggerHelper, LoggerHelper>();
        services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
        services.AddSingleton<IJsonHelper, JsonHelper>();
        services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
        services.AddScoped<ISubscriptionHelper, SubscriptionHelper>();
        services.AddScoped<IGetCustomerByIdHttpTriggerService, GetCustomerByIdHttpTriggerService>();
        services.AddScoped<IPostCustomerHttpTriggerService, PostCustomerHttpTriggerService>();
        services.AddScoped<IPatchCustomerHttpTriggerService, PatchCustomerHttpTriggerService>();
        services.AddScoped<ICustomerPatchService, CustomerPatchService>();
        services.AddScoped<IServiceBusClient, ServiceBusClient>();
        services.AddSingleton<IDocumentDBProvider, DocumentDBProvider>();
        services.AddSingleton<IDynamicHelper, DynamicHelper>();
    })
    .Build();

host.Run();
