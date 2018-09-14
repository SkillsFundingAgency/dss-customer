using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Models
{
    public class Subscriptions
    {
        public Guid? CustomerId { get; set; }
        public Guid SubscriptionId { get; set; }
        public string TouchPointId { get; set; }
        public bool Subscribe { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}