using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NCS.DSS.Customer;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.ServiceBus;
using NCS.DSS.Customer.Validation;
[assembly: FunctionsStartup(typeof(Startup))]

namespace NCS.DSS.Customer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton<IResourceHelper, ResourceHelper>();
            builder.Services.AddSingleton<IValidate, Validate>();
            builder.Services.AddSingleton<ILoggerHelper, LoggerHelper>();
            builder.Services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
            builder.Services.AddSingleton<IHttpResponseMessageHelper, HttpResponseMessageHelper>();
            builder.Services.AddSingleton<IJsonHelper, JsonHelper>();
            builder.Services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
            builder.Services.AddScoped<ISubscriptionHelper, SubscriptionHelper>();
            builder.Services.AddScoped<IGetCustomerByIdHttpTriggerService, GetCustomerByIdHttpTriggerService>();
            builder.Services.AddScoped<IPostCustomerHttpTriggerService, PostCustomerHttpTriggerService>();
            builder.Services.AddScoped<IPatchCustomerHttpTriggerService, PatchCustomerHttpTriggerService>();
            builder.Services.AddScoped<ICustomerPatchService, CustomerPatchService>();
            builder.Services.AddScoped<IServiceBusClient, ServiceBusClient>();
            builder.Services.AddSingleton<IDocumentDBProvider, DocumentDBProvider>();
        }
    }
}


