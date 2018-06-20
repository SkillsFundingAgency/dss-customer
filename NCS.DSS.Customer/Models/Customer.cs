using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using NCS.DSS.Customer.ReferenceData;

namespace NCS.DSS.Customer.Models
{
    public class Customer
    {
        [Required]
        [Display(Description = "Unique identifier of a customer")]
        public Guid CustomerID { get; set; }

        [Display(Description = "Date and time the customer was first recognised by the National Careers Service")]
        public DateTime DateOfRegistration { get; set; }

        [Display(Description = "See DSS Reference Data Resource for values")]
        public int TitleID { get; set; }

        [Display(Description = "Customers first or given name")]
        [MaxLength(100)]
        public string GivenName { get; set; }

        [Display(Description = "Customers family or surname")]
        [MaxLengthAttribute(100)]
        public string FamilyName { get; set; }

        [Display(Description = "Customers date of birth")]
        public DateTime DateofBirth { get; set; }

        [Display(Description = "See DSS Reference Data Resource for values.")]
        public Gender GenderID { get; set; }

        [Display(Description = "Customers unique learner number as issued by the learning record service")]
        [MaxLengthAttribute(10)]
        public string UniqueLearnerNumber { get; set; }

        [Display(Description = "An indicator to show whether an individual wishes to participate in User Research or not")]
        public bool OptInUserResearch { get; set; }

        [Display(Description = "An indicator to show whether an individual wishes to participate in Market Research or not")]
        public bool OptInMarketResearch { get; set; }

        [Display(Description = "Date the customer closed their account")]
        public DateTime DateOfAccountClosure { get; set; }

        [Display(Description = "Reason for why the customer closed their account.  See DSS Reference Data Resource for values")]
        public ReasonForClosure ReasonForClosureID { get; set; }

        [Display(Description = "See DSS Reference Data Resource for values")]
        public IntroducedBy IntroducedByID { get; set; }

        [Display(Description = "Additional information on how the customer was introduced to the National Careers Service")]
        public string IntroducedByAdditionalInfo { get; set; }

        [Display(Description = "Date and time of the last modification to the record")]
        public DateTime LastModifiedDate { get; set; }

        [Display(Description = "Identifier of the touchpoint who made the last change to the record")]
        public Guid LastModifiedTouchpointID { get; set; }

    }
}
