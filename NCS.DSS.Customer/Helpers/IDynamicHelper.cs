using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Helpers
{
    public interface IDynamicHelper
    {
        public void AddProperty(ExpandoObject expando, string propertyName, object propertyValue);
        public ExpandoObject ExcludeProperty(Exception exception, string[] names);
    }
}
