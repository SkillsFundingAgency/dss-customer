﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.GetCustomerHttpTrigger
{
    class GetCustomerHttpTriggerService
    {
        public async Task<List<Models.Customer>> GetCustomer()
        {
            var result = CreateTempCustomers();
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
                    DateofBirth = Convert.ToDateTime("01/04/1940"),
                    ReferenceData = new Data.ReferenceData()
                    
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "America",
                    GivenName = "Captain",
                    DateofBirth = Convert.ToDateTime("01/04/1920"),
                    ReferenceData = new Data.ReferenceData()
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "Man",
                    GivenName = "Iron",
                    DateofBirth = Convert.ToDateTime("01/04/1940"),
                    ReferenceData = new Data.ReferenceData()
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "Trump",
                    GivenName = "Donald",
                    DateofBirth = Convert.ToDateTime("01/04/1950"),
                    ReferenceData = new Data.ReferenceData()
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "Mao",
                    GivenName = "Chairman",
                    DateofBirth = Convert.ToDateTime("01/04/1897"),
                    ReferenceData = new Data.ReferenceData()
                },
                new Models.Customer
                {
                    CustomerID = Guid.NewGuid(),
                    FamilyName = "Putin",
                    GivenName = "Vladimir",
                    DateofBirth = Convert.ToDateTime("01/04/1957"),
                    ReferenceData = new Data.ReferenceData()
                }
            };

            return CustList;
        }



    }
}
