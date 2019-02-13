using NCS.DSS.Customer.Models;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public class CustomerPatchService : ICustomerPatchService
    {       
        public Models.Customer Patch(Models.Customer customer, CustomerPatch customerPatch)
        {
            if (customerPatch == null)
                return null;

            if (!string.IsNullOrEmpty(customerPatch.SubcontractorId))
                customer.SubcontractorId = customerPatch.SubcontractorId;

            if (customerPatch.DateOfRegistration.HasValue)
                customer.DateOfRegistration = customerPatch.DateOfRegistration;

            if (customerPatch.Title.HasValue)
                customer.Title = customerPatch.Title;

            if (!string.IsNullOrEmpty(customerPatch.GivenName))
                customer.GivenName = customerPatch.GivenName;

            if (!string.IsNullOrEmpty(customerPatch.FamilyName))
                customer.FamilyName = customerPatch.FamilyName;

            if (customerPatch.DateofBirth.HasValue)
                customer.DateofBirth = customerPatch.DateofBirth;

            if (customerPatch.Gender.HasValue)
                customer.Gender = customerPatch.Gender;

            if (!string.IsNullOrEmpty(customerPatch.UniqueLearnerNumber))
                customer.UniqueLearnerNumber = customerPatch.UniqueLearnerNumber;

            if (customerPatch.OptInUserResearch.HasValue)
                customer.OptInUserResearch = customerPatch.OptInUserResearch;

            if (customerPatch.OptInMarketResearch.HasValue)
                customer.OptInMarketResearch = customerPatch.OptInMarketResearch;

            if (customerPatch.DateOfTermination.HasValue)
                customer.DateOfTermination = customerPatch.DateOfTermination;

            if (customerPatch.ReasonForTermination.HasValue)
                customer.ReasonForTermination = customerPatch.ReasonForTermination;

            if (customerPatch.IntroducedBy.HasValue)
                customer.IntroducedBy = customerPatch.IntroducedBy;

            if (!string.IsNullOrEmpty(customerPatch.IntroducedByAdditionalInfo))
                customer.IntroducedByAdditionalInfo = customerPatch.IntroducedByAdditionalInfo;

            if (customerPatch.LastModifiedDate.HasValue)
                customer.LastModifiedDate = customerPatch.LastModifiedDate;

            if (!string.IsNullOrEmpty(customerPatch.LastModifiedTouchpointId))
                customer.LastModifiedTouchpointId = customerPatch.LastModifiedTouchpointId;

            return customer;
        }
    }
}