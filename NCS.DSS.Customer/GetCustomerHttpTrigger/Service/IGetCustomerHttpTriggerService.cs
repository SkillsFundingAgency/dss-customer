using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.GetCustomerHttpTrigger.Service
{
    public interface IGetCustomerHttpTriggerService
    {
        Task<List<Models.Customer>> GetAllCustomerAsync();
    }
}
