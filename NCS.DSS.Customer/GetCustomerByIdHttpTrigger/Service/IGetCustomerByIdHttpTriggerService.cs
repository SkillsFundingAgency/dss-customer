using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service
{
    public interface IGetCustomerByIdHttpTriggerService
    {
        Task<Models.Customer> GetCustomer(Guid customerId);
    }
}
