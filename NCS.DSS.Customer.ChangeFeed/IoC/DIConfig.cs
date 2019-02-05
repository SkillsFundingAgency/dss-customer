using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NCS.DSS.Customer.ChangeFeed.CustomerChangeFeedTrigger.Service;
using NCS.DSS.Customer.ChangeFeed.IoC;
using NCS.DSS.Customer.ChangeFeed.SQLServer;
using System;
using System.Data;
using System.Data.SqlClient;

[assembly: WebJobsStartup(typeof(DIConfig))]

namespace NCS.DSS.Customer.ChangeFeed.IoC
{
    public class DIConfig : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection();

            RegisterServices(builder);
            RegisterHelpers(builder);            
            RegisterSQLServer(builder);
        }

        private void RegisterServices(IWebJobsBuilder builder)
        {
            builder.Services.AddScoped<ICustomerChangeFeedTriggerService, CustomerChangeFeedTriggerService>();
        }

        private void RegisterHelpers(IWebJobsBuilder builder)
        {
            builder.Services.AddScoped<ILoggerHelper, LoggerHelper>();            
        }

        private void RegisterSQLServer(IWebJobsBuilder builder)
        {
            builder.Services.AddScoped<ISQLServerProvider, SQLServerProvider>();
            builder.Services.AddScoped<IDbConnection>(db => new SqlConnection(Environment.GetEnvironmentVariable("SQLConnString")));
        }
    }
}
