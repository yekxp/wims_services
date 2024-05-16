using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace analytical_management.Model
{
    public class Buyer
    {
        [JsonPropertyName("buyerId")]
        [JsonProperty("buyerId")]
        public string BuyerId { get; set; }

        [JsonPropertyName("buyerName")]
        [JsonProperty("buyerName")]
        public  string BuyerName { get; set; }

        [JsonPropertyName("buyerSurname")]
        [JsonProperty("buyerSurname")]
        public  string BuyerSurname { get; set; }

        [JsonPropertyName("buyerPhoneNumber")]
        [JsonProperty("buyerPhoneNumber")]
        public  string BuyerPhoneNumber { get; set; }

        [JsonPropertyName("buyerEmail")]
        [JsonProperty("buyerEmail")]
        public  string BuyerEmail { get; set; }

        [JsonPropertyName("buyerCountry")]
        [JsonProperty("buyerCountry")]
        public  string BuyerCountry { get; set; }

        [JsonPropertyName("buyerCity")]
        [JsonProperty("buyerCity")]
        public  string BuyerCity { get; set; }

        [JsonPropertyName("buyerZipCode")]
        [JsonProperty("buyerZipCode")]
        public  string BuyerZipCode { get; set; }

        [JsonPropertyName("buyerAddress")]
        [JsonProperty("buyerAddress")]
        public  string BuyerAddress { get; set; }
    }
}
