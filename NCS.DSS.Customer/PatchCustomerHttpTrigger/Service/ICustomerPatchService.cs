using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public interface ICustomerPatchService
    {
        Models.Customer Patch(string customerJson, CustomerPatch customerPatch);
    }
}
