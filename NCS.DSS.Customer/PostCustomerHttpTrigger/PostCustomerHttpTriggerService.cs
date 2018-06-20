using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.PostCustomerHttpTrigger
{
    class PostCustomerHttpTriggerService
    {
        public Guid? Create(Models.Customer customer)
        {
            if (customer == null)
                return null;

            return Guid.NewGuid();
        }
    }
}
