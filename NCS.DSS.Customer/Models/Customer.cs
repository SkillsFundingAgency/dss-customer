using NCS.DSS.Customer.ReferenceData;
using System;
using System.ComponentModel.DataAnnotations;
using DFC.JSON.Standard.Attributes;
using DFC.Swagger.Standard.Annotations;
using System.Collections.Generic;
using Newtonsoft.Json;
using NCS.DSS.Customer.Helpers;

namespace NCS.DSS.Customer.Models
{
    public class Customer : ICustomer
    {
        [Display(Description = "Unique identifier of a customer")]
        [Example(Description = "b8592ff8-af97-49ad-9fb2-e5c3c717fd85")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public Guid? CustomerId { get; set; }

        [Display(Description = "Date and time the customer was first recognised by the National Careers Service")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? DateOfRegistration { get; set; }

        [Display(Description = "Customers given title.")]
        [Example(Description = "1")]
        public Title? Title { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z ]+((['\,\.\- ][a-zA-Z ])?[a-zA-Z ]*)*$")]
        [Display(Description = "Customers first or given name")]
        [Example(Description = "Boris")]
        [StringLength(100)]
        public string GivenName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z ]+((['\,\.\- ][a-zA-Z ])?[a-zA-Z ]*)*$")]
        [Display(Description = "Customers family or surname")]
        [Example(Description = "Johnson")]
        [StringLength(100)]
        public string FamilyName { get; set; }

        [Display(Description = "Customers date of birth")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? DateofBirth { get; set; }

        [Display(Description = "Customers gender.")]
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

        [Display(Description = "Reason for why the customer terminated their account.")]
        [Example(Description = "3")]
        public ReasonForTermination? ReasonForTermination { get; set; }

        [Display(Description = "Introduced By.")]
        [Example(Description = "12345")]
        public IntroducedBy? IntroducedBy { get; set; }

        [Display(Description = "Additional information on how the customer was introduced to the National Careers Service")]
        [Example(Description = "Customer was introduced to NCS by party X on date Y")]
        [StringLength(100)]
        public string IntroducedByAdditionalInfo { get; set; }

        [StringLength(50)]
        [Display(Description = "Identifier supplied by the touchpoint to indicate their subcontractor")]
        [Example(Description = "01234567899876543210")]
        public string SubcontractorId { get; set; }

        [Display(Description = "Date and time of the last modification to the record")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? LastModifiedDate { get; set; }

        [StringLength(10, MinimumLength = 10)]
        [Display(Description = "Identifier of the touchpoint who made the last change to the record")]
        [Example(Description = "0000000001")]
        public string LastModifiedTouchpointId { get; set; }

        [Required]
        [Display(Description = "Priority Customer reference data.")]
        [Example(Description = "[2,3]")]
        [JsonConverter(typeof(PriorityGroupConverter))]
        public List<PriorityCustomer> PriorityGroups { get; set; }

        [JsonIgnoreOnSerialize]
        public string CreatedBy { get; set; }

        public void SetDefaultValues()
        {
            var customerId = Guid.NewGuid();
            CustomerId = customerId;

            if (!DateOfRegistration.HasValue)
                DateOfRegistration = DateTime.UtcNow;

            if (!LastModifiedDate.HasValue)
                LastModifiedDate = DateTime.UtcNow;

            if (!OptInUserResearch.HasValue)
                OptInUserResearch = false;

            if (!OptInMarketResearch.HasValue)
                OptInMarketResearch = false;

            if (Title == null)
                Title = ReferenceData.Title.NotProvided;

            if (Gender == null)
                Gender = ReferenceData.Gender.NotProvided;

            if (DateOfTermination.HasValue && ReasonForTermination == null)
                ReasonForTermination = ReferenceData.ReasonForTermination.Other;

            if (IntroducedBy == null)
                IntroducedBy = ReferenceData.IntroducedBy.NotProvided;

        }

        public void SetIds(string touchpointId, string subcontractorid)
        {
            LastModifiedTouchpointId = touchpointId;
            SubcontractorId = subcontractorid;
            CreatedBy = touchpointId;
        }
    }    
}
