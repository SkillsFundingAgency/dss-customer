using Azure.Identity;
using Azure.Messaging.ServiceBus;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCS.DSS.Customer.Cosmos.Provider;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Models;
using NCS.DSS.Customer.PatchCustomerHttpTrigger.Service;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.ServiceBus;
using NCS.DSS.Customer.Validation;
namespace NCS.DSS.Customer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("local.settings.json", optional: true,
                            reloadOnChange: false)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((context,services) =>
                {
                    var configuration = context.Configuration;
                    services.AddOptions<CustomerConfigurationSettings>()
                        .Bind(configuration);
                    services.AddLogging();
                    services.AddApplicationInsightsTelemetryWorkerService(); 
                    services.ConfigureFunctionsApplicationInsights();
                    services.AddSingleton<IValidate, Validate>();        
                    services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
                    services.AddSingleton<IJsonHelper, JsonHelper>();
                    services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
                    services.AddScoped<IGetCustomerByIdHttpTriggerService, GetCustomerByIdHttpTriggerService>();
                    services.AddScoped<IPostCustomerHttpTriggerService, PostCustomerHttpTriggerService>();
                    services.AddScoped<IPatchCustomerHttpTriggerService, PatchCustomerHttpTriggerService>();
                    services.AddScoped<ICustomerPatchService, CustomerPatchService>();
                    services.AddScoped<ICustomerServiceBusClient, CustomerServiceBusClient>();
                    services.AddTransient<ICosmosDBProvider, CosmosDBProvider>();
                    services.AddSingleton(sp =>
                    {
                        var cosmosDbEndpoint = configuration["CosmosDbEndpoint"];
                        if (string.IsNullOrEmpty(cosmosDbEndpoint))
                        {
                            throw new InvalidOperationException("CosmosDbEndpoint is not configured.");
                        }

                        var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
                        return new CosmosClient(cosmosDbEndpoint, new DefaultAzureCredential(), options);
                    });
                    services.AddSingleton(serviceProvider =>
                    {
                        var settings = serviceProvider.GetRequiredService<IOptions<CustomerConfigurationSettings>>().Value;
                        return new ServiceBusClient(settings.ServiceBusConnectionString);
                    });
                    services.AddSingleton<IDynamicHelper, DynamicHelper>();
                    services.Configure<LoggerFilterOptions>(options =>
                    {
                        LoggerFilterRule toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
                            == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
                        if (toRemove is not null)
                        {
                            options.Rules.Remove(toRemove);
                        }
                    });
                })
                .Build();

            await host.RunAsync();
        }
    }

}

