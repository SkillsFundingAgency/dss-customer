using DFC.JSON.Standard;
using NCS.DSS.Customer.Models;
using Newtonsoft.Json.Linq;

namespace NCS.DSS.Customer.PatchCustomerHttpTrigger.Service
{
    public class CustomerPatchService : ICustomerPatchService
    {

        private readonly IJsonHelper _jsonHelper;

        public CustomerPatchService(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public Models.Customer Patch(string customerJson, CustomerPatch customerPatch)
        {
            if (string.IsNullOrEmpty(customerJson))
                return null;

            var obj = JObject.Parse(customerJson);
            
            if (!string.IsNullOrEmpty(customerPatch.SubcontractorId))
            {
                if (obj["SubcontractorId"] == null)
                    _jsonHelper.CreatePropertyOnJObject(obj, "SubcontractorId", customerPatch.SubcontractorId);
                else
                    _jsonHelper.UpdatePropertyValue(obj["SubcontractorId"], customerPatch.SubcontractorId);
            }

            if (customerPatch.DateOfRegistration.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["DateOfRegistration"], customerPatch.DateOfRegistration);

            if (customerPatch.Title.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["Title"], customerPatch.Title);

            if (!string.IsNullOrEmpty(customerPatch.GivenName))
                _jsonHelper.UpdatePropertyValue(obj["GivenName"], customerPatch.GivenName);

            if (!string.IsNullOrEmpty(customerPatch.FamilyName))
                _jsonHelper.UpdatePropertyValue(obj["FamilyName"], customerPatch.FamilyName);

            if (customerPatch.DateofBirth.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["DateofBirth"], customerPatch.DateofBirth);

            if (customerPatch.Gender.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["Gender"], customerPatch.Gender);

            if (!string.IsNullOrEmpty(customerPatch.UniqueLearnerNumber))
                _jsonHelper.UpdatePropertyValue(obj["UniqueLearnerNumber"], customerPatch.UniqueLearnerNumber);

            if (customerPatch.OptInUserResearch.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["OptInUserResearch"], customerPatch.OptInUserResearch);

            if (customerPatch.OptInMarketResearch.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["OptInMarketResearch"], customerPatch.OptInMarketResearch);

            if (customerPatch.DateOfTermination.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["DateOfTermination"], customerPatch.DateOfTermination);

            if (customerPatch.ReasonForTermination.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["ReasonForTermination"], customerPatch.ReasonForTermination);

            if (customerPatch.IntroducedBy.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["IntroducedBy"], customerPatch.IntroducedBy);

            if (!string.IsNullOrEmpty(customerPatch.IntroducedByAdditionalInfo))
                _jsonHelper.UpdatePropertyValue(obj["IntroducedByAdditionalInfo"], customerPatch.IntroducedByAdditionalInfo);

            if (customerPatch.LastModifiedDate.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["LastModifiedDate"], customerPatch.LastModifiedDate);

            if (!string.IsNullOrEmpty(customerPatch.LastModifiedTouchpointId))
                _jsonHelper.UpdatePropertyValue(obj["LastModifiedTouchpointId"], customerPatch.LastModifiedTouchpointId);

            return obj.ToObject<Models.Customer>();

        }
    }
}