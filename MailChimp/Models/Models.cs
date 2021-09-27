using Newtonsoft.Json;
using MailChimp.Net.Models;

namespace MailChimp.Models
{
    public class SubscribeUserModel
    {
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }

    public class UnsubscribeUserModel
    {
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }

    public class AddUserTagsModel
    {
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }
        [JsonProperty("tags")]
        public Tag[] Tags { get; set; }
    }
}
