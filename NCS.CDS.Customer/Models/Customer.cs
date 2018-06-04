using System;
using System.ComponentModel.DataAnnotations;

namespace NCS.CDS.Customer.Models
{
    public class Customer
    {
        public Guid CustomerID { get; set; }

        public DateTime DateOfRegistration { get; set; }

        public int TitleID { get; set; }

        [StringLength(100)]
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
