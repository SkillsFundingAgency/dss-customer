﻿using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NCS.DSS.Customer.Ioc;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.SearchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.ServiceBus;
using NCS.DSS.Customer.Validation;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace NCS.DSS.Customer.Ioc
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection();

            RegisterHelpers(builder);
            RegisterServices(builder);
            RegisterDataProviders(builder);
        }

        private void RegisterHelpers(IWebJobsBuilder builder)
        {
            builder.Services.AddSingleton<IResourceHelper, ResourceHelper>();
            builder.Services.AddSingleton<IValidate, Validate>();
            builder.Services.AddSingleton<ILoggerHelper, LoggerHelper>();
            builder.Services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
            builder.Services.AddSingleton<IHttpResponseMessageHelper, HttpResponseMessageHelper>();
            builder.Services.AddSingleton<IJsonHelper, JsonHelper>();
            builder.Services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();               
            builder.Services.AddScoped<ISubscriptionHelper, SubscriptionHelper>();               
        }

        private void RegisterServices(IWebJobsBuilder builder)
        {
            builder.Services.AddScoped<IGetCustomerByIdHttpTriggerService, GetCustomerByIdHttpTriggerService>();
            builder.Services.AddScoped<IPostCustomerHttpTriggerService, PostCustomerHttpTriggerService>();
            builder.Services.AddScoped<IPatchCustomerHttpTriggerService, PatchCustomerHttpTriggerService>();
            builder.Services.AddScoped<ISearchCustomerHttpTriggerService, SearchCustomerHttpTriggerService>();            
            builder.Services.AddScoped<ICustomerPatchService, CustomerPatchService>();
            builder.Services.AddScoped<IServiceBusClient, ServiceBusClient>();

        }

        private void RegisterDataProviders(IWebJobsBuilder builder)
        {
            builder.Services.AddSingleton<IDocumentDBProvider, DocumentDBProvider>();            
        }
    }
}