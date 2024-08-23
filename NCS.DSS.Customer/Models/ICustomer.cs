using NCS.DSS.Customer.ReferenceData;

namespace NCS.DSS.Customer.Models
{
    public interface ICustomer
    {
        DateTime? DateOfRegistration { get; set; }
        Title? Title { get; set; }
        string GivenName { get; set; }
        string FamilyName { get; set; }
        DateTime? DateofBirth { get; set; }
        Gender? Gender { get; set; }
        string UniqueLearnerNumber { get; set; }
        bool? OptInUserResearch { get; set; }
        bool? OptInMarketResearch { get; set; }
        DateTime? DateOfTermination { get; set; }
        ReasonForTermination? ReasonForTermination { get; set; }
        IntroducedBy? IntroducedBy { get; set; }
        string IntroducedByAdditionalInfo { get; set; }
        DateTime? LastModifiedDate { get; set; }
        string LastModifiedTouchpointId { get; set; }
        List<PriorityCustomer> PriorityGroups { get; set; }

        void SetDefaultValues();
    }
}