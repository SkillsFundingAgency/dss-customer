using NCS.DSS.Customer.Cosmos.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.GetCustomerHttpTrigger.Service
{
    public class GetCustomerHttpTriggerService : IGetCustomerHttpTriggerService
    {
        private readonly IDocumentDBProvider _documentDbProvider;
        public GetCustomerHttpTriggerService(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }
        public async Task<List<Models.Customer>> GetAllCustomerAsync()
        {
            return await _documentDbProvider.GetAllCustomer();         
        }
    }
}
