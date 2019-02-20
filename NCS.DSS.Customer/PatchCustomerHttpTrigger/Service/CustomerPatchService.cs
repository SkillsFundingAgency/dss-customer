using NCS.DSS.Customer.Helpers;
using NCS.DSS.Customer.Models;
using Newtonsoft.Json.Linq;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public class CustomerPatchService : ICustomerPatchService
    {

        public Models.Customer Patch(string customerJson, CustomerPatch customerPatch)
        {
            if (string.IsNullOrEmpty(customerJson))
                return null;

            var obj = JObject.Parse(customerJson);

            if (customerPatch.DateOfRegistration.HasValue)
                JsonHelper.UpdatePropertyValue(obj["DateOfRegistration"], customerPatch.DateOfRegistration);

            if (customerPatch.Title.HasValue)
                JsonHelper.UpdatePropertyValue(obj["Title"], customerPatch.Title);

            if (!string.IsNullOrEmpty(customerPatch.GivenName))
                JsonHelper.UpdatePropertyValue(obj["GivenName"], customerPatch.GivenName);

            if (!string.IsNullOrEmpty(customerPatch.FamilyName))
                JsonHelper.UpdatePropertyValue(obj["FamilyName"], customerPatch.FamilyName);

            if (customerPatch.DateofBirth.HasValue)
                JsonHelper.UpdatePropertyValue(obj["DateofBirth"], customerPatch.DateofBirth);

            if (customerPatch.Gender.HasValue)
                JsonHelper.UpdatePropertyValue(obj["Gender"], customerPatch.Gender);

            if (!string.IsNullOrEmpty(customerPatch.UniqueLearnerNumber))
                JsonHelper.UpdatePropertyValue(obj["UniqueLearnerNumber"], customerPatch.UniqueLearnerNumber);

            if (customerPatch.OptInUserResearch.HasValue)
                JsonHelper.UpdatePropertyValue(obj["OptInUserResearch"], customerPatch.OptInUserResearch);

            if (customerPatch.OptInMarketResearch.HasValue)
                JsonHelper.UpdatePropertyValue(obj["OptInMarketResearch"], customerPatch.OptInMarketResearch);

            if (customerPatch.DateOfTermination.HasValue)
                JsonHelper.UpdatePropertyValue(obj["DateOfTermination"], customerPatch.DateOfTermination);

            if (customerPatch.ReasonForTermination.HasValue)
                JsonHelper.UpdatePropertyValue(obj["ReasonForTermination"], customerPatch.ReasonForTermination);

            if (customerPatch.IntroducedBy.HasValue)
                JsonHelper.UpdatePropertyValue(obj["IntroducedBy"], customerPatch.IntroducedBy);

            if (!string.IsNullOrEmpty(customerPatch.IntroducedByAdditionalInfo))
                JsonHelper.UpdatePropertyValue(obj["IntroducedByAdditionalInfo"], customerPatch.IntroducedByAdditionalInfo);

            if (customerPatch.LastModifiedDate.HasValue)
                JsonHelper.UpdatePropertyValue(obj["LastModifiedDate"], customerPatch.LastModifiedDate);

            if (!string.IsNullOrEmpty(customerPatch.LastModifiedTouchpointId))
                JsonHelper.UpdatePropertyValue(obj["LastModifiedTouchpointId"], customerPatch.LastModifiedTouchpointId);

            return obj.ToObject<Models.Customer>();

        }
    }
}