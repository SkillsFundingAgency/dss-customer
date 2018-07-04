using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger
{
    class PostCustomerHttpTriggerService
    {
        public Models.Customer CreateNewCustomer()
        {
            Models.Customer newCust = new Models.Customer();
            newCust.CustomerID = Guid.NewGuid();
            return newCust;
        }
    }
}
