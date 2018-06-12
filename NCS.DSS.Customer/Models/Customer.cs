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

namespace NCS.DSS.Customer.Models
{
    public class Customer
    {
        [Required]
        public Guid CustomerID { get; set; }

        public DateTime DateOfRegistration { get; set; }

        public int TitleID { get; set; }

        [StringLengthAttribute(100)]
        public string GivenName { get; set; }

        [StringLength(100)]
        public string FamilyName { get; set; }

        public DateTime DateofBirth { get; set; }

        public int GenderID { get; set; }

        [StringLength(10)]
        public string UniqueLearnerNumber { get; set; }

        public bool OptInUserResearch { get; set; }

        public bool OptInMarketResearch { get; set; }

        public DateTime DateOfAccountClosure { get; set; }

        public int ReasonForClosureID { get; set; }

        public int IntroducedByID { get; set; }

        public string IntroducedByAdditionalInfo { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public Guid LastModifiedTouchpointID { get; set; }

    }
}
