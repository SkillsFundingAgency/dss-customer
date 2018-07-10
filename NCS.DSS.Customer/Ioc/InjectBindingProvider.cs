using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using NCS.DSS.Customer.Cosmos.Helper;
using NCS.DSS.Customer.GetCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.PostCustomerHttpTrigger.Service;
using NCS.DSS.Customer.Validation;

namespace NCS.DSS.Customer.Ioc
{
    internal class InjectBindingProvider : IBindingProvider
    {
        public static readonly ConcurrentDictionary<Guid, IServiceScope> Scopes =
            new ConcurrentDictionary<Guid, IServiceScope>();

        private IServiceProvider _serviceProvider;

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (_serviceProvider == null)
                _serviceProvider = CreateServiceProvider();

            IBinding binding = new InjectBinding(_serviceProvider, context.Parameter.ParameterType);
            return Task.FromResult(binding);
        }

        private static IServiceProvider CreateServiceProvider()
        {
            var registerServicesProviders = new RegisterServiceProvider();
            return registerServicesProviders.CreateServiceProvider();
        }
    }
}
