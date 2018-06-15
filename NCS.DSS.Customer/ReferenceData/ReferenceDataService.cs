using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Data
{
    class ReferenceDataService
    {
        public List<string> GetReferenceDataTypes()
        {
            List<string> ReferenceDataTypes = new List<string>();
            ReferenceData nRef = new ReferenceData();
            foreach (var prop in nRef.GetType().GetProperties())
            {

                    ReferenceDataTypes.Add(prop.Name);

            }

            return ReferenceDataTypes;
        }
    }
}
