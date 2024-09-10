namespace NCS.DSS.Customer.GetCustomerByIdHttpTrigger.Service
{
    public interface IGetCustomerByIdHttpTriggerService
    {
        Task<Models.Customer> GetCustomerAsync(Guid customerId);
    }
}
