using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.SearchCustomerHttpTrigger.Service
{
    public interface ISearchCustomerHttpTriggerService
    {
        Task<List<Models.Customer>> SearchCustomerAsync(string qQuery);
    }
}
