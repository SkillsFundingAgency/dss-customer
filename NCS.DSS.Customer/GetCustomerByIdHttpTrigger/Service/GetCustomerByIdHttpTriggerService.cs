using NCS.DSS.Customer.Cosmos.Provider;

namespace NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service
{
    public class GetCustomerByIdHttpTriggerService : IGetCustomerByIdHttpTriggerService
    {
        private readonly IDocumentDBProvider _documentDbProvider;
        public GetCustomerByIdHttpTriggerService(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }
        public async Task<Models.Customer> GetCustomerAsync(Guid customerId)
        {
            return await _documentDbProvider.GetCustomerByIdAsync(customerId);
        }

        public List<Models.Customer> CreateTempCustomers()
        {
            var CustList = new List<Models.Customer>
            {
                new Models.Customer
                {
                    CustomerId = Guid.Parse("80934cac-3a38-419e-b35a-4d934c6d1cb9"),
                    FamilyName = "Burns",
                    GivenName = "Montgomery",
                    DateofBirth = Convert.ToDateTime("01/04/1940")
                },
                new Models.Customer
                {
                    CustomerId = Guid.Parse("4eb5637c-08bb-4cc2-92d3-45a0ce7804ad"),
                    FamilyName = "America",
                    GivenName = "Captain",
                    DateofBirth = Convert.ToDateTime("01/04/1920")
                },
                new Models.Customer
                {
                    CustomerId = Guid.Parse("c09739ed-182a-41b3-ac50-51991704ff3c"),
                    FamilyName = "Man",
                    GivenName = "Iron",
                    DateofBirth = Convert.ToDateTime("01/04/1940")
                },
                new Models.Customer
                {
                    CustomerId = Guid.Parse("24621677-912b-4f6c-b387-7070d9d852d4"),
                    FamilyName = "Trump",
                    GivenName = "Donald",
                    DateofBirth = Convert.ToDateTime("01/04/1950")
                },
                new Models.Customer
                {
                    CustomerId = Guid.Parse("f9580cc5-e148-4dce-b3d5-6ebb765936a5"),
                    FamilyName = "Mao",
                    GivenName = "Chairman",
                    DateofBirth = Convert.ToDateTime("01/04/1897")
                },
                new Models.Customer
                {
                    CustomerId = Guid.Parse("c7c10b8e-c4e3-4afe-aef0-a365514da086"),
                    FamilyName = "Putin",
                    GivenName = "Vladimir",
                    DateofBirth = Convert.ToDateTime("01/04/1957")
                }
            };

            return CustList;
        }

    }
}
