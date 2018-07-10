using System;
using Microsoft.Extensions.DependencyInjection;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service;
using NCS.DSS.Customer.GetCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.PostCustomerHttpTrigger;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;

namespace NCS.DSS.Customer.Ioc
{
    public class RegisterServiceProvider
    {
        public IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddTransient<IGetCustomerByIdHttpTriggerService, GetCustomerByIdHttpTriggerService>();
            services.AddTransient<IPostCustomerHttpTriggerService, PostCustomerHttpTriggerService>();

            services.AddTransient<IResourceHelper, ResourceHelper>();
            services.AddTransient<IValidate, Validate>();
            services.AddTransient<IHttpRequestMessageHelper, HttpRequestMessageHelper>();
            return services.BuildServiceProvider(true);
        }
    }
}
