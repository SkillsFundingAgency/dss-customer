using System;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.Customer.Annotations;
using NCS.DSS.Customer.ReferenceData;

namespace NCS.DSS.Customer.Models
{
    public class CustomerPatch
    {

        [Display(Description = "Date and time the customer was first recognised by the National Careers Service")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? DateOfRegistration { get; set; }

        [Display(Description = "Customers given title")]
        [Example(Description = "1")]
        public Title Title { get; set; }

        [Display(Description = "Customers first or given name")]
        [Example(Description = "Boris")]
        [StringLength(100)]
        public string GivenName { get; set; }

        [Display(Description = "Customers family or surname")]
        [Example(Description = "Johnson")]
        [StringLength(100)]
        public string FamilyName { get; set; }

        [Display(Description = "Customers date of birth")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? DateofBirth { get; set; }

        [Display(Description = "Customers gender")]
        [Example(Description = "3")]
        public Gender? Gender { get; set; }

        [Display(Description = "Customers unique learner number as issued by the learning record service")]
        [Example(Description = "3000000000")]
        [StringLength(10)]
        public string UniqueLearnerNumber { get; set; }

        [Display(Description = "An indicator to show whether an individual wishes to participate in User Research or not")]
        [Example(Description = "true/false")]
        public bool? OptInUserResearch { get; set; }

        [Display(Description = "An indicator to show whether an individual wishes to participate in Market Research or not")]
        [Example(Description = "true/false")]
        public bool? OptInMarketResearch { get; set; }

        [Display(Description = "Date the customer terminated their account")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? DateOfTermination { get; set; }

        [Display(Description = "Reason for why the customer terminated their account.  See DSS Reference Data Resource for values")]
        [Example(Description = "3")]
        [StringLength(100)]
        public ReasonForTermination? ReasonForTermination { get; set; }

        [Display(Description = "See DSS Reference Data Resource for values")]
        [Example(Description = "12345")]
        public IntroducedBy? IntroducedBy { get; set; }

        [Display(Description = "Additional information on how the customer was introduced to the National Careers Service")]
        [Example(Description = "Customer was introduced to NCS by party X on date Y")]
        public string IntroducedByAdditionalInfo { get; set; }

        [Display(Description = "Date and time of the last modification to the record")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? LastModifiedDate { get; set; }

        [Display(Description = "Identifier of the touchpoint who made the last change to the record")]
        [Example(Description = "b8592ff8-af97-49ad-9fb2-e5c3c717fd85")]
        public Guid? LastModifiedTouchpointId { get; set; }

        public void SetDefaultValues()
        {
            if (!LastModifiedDate.HasValue)
                LastModifiedDate = DateTime.Now;

            if (DateOfTermination.HasValue && ReasonForTermination == null)
                ReasonForTermination = ReferenceData.ReasonForTermination.Other;
        }
    }
}
