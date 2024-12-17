using Newtonsoft.Json;

namespace NCS.DSS.Customer.Models
{
    public class Subscriptions
    {
        public Guid? CustomerId { get; set; }
        [JsonProperty(PropertyName = "id")]
        public Guid SubscriptionId { get; set; }
        public string TouchPointId { get; set; }
        public bool Subscribe { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}