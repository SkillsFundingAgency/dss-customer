using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.CDS.Customer.GetCustomerByIdHttpTrigger
{
    class GetCustomerByIdHttpTriggerService
    {
        public async Task<List<Models.Customer>> GetCustomer(Guid customerId)
        {
            var result = CreateTempCustomers();
            result.FirstOrDefault(x => x.CustomerID == customerId);
            return await Task.FromResult(result);
        }

        public List<Models.Customer> CreateTempCustomers()
        {
            var CustList = new List<Models.Customer>
            {
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "Burns",
                    GivenName = "Montgomery",
                    DateofBirth = Convert.ToDateTime("01/04/1940")
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "America",
                    GivenName = "Captain",
                    DateofBirth = Convert.ToDateTime("01/04/1920")
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "Man",
                    GivenName = "Iron",
                    DateofBirth = Convert.ToDateTime("01/04/1940")
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "Trump",
                    GivenName = "Donald",
                    DateofBirth = Convert.ToDateTime("01/04/1950")
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "Mao",
                    GivenName = "Chairman",
                    DateofBirth = Convert.ToDateTime("01/04/1897")
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "Putin",
                    GivenName = "Vladimir",
                    DateofBirth = Convert.ToDateTime("01/04/1957")
                }
            };

            return CustList;
        }



    }
}
