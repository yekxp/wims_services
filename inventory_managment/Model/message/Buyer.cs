using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace inventory_managment.Model.message
{
    public class Buyer
    {
        [JsonPropertyName("buyerId")]
        [JsonProperty("buyerId")]
        public string BuyerId { get; set; }

        [JsonPropertyName("buyerName")]
        [JsonProperty("buyerName")]
        public required string BuyerName { get; set; }

        [JsonPropertyName("buyerSurname")]
        [JsonProperty("buyerSurname")]
        public required string BuyerSurname { get; set; }

        [JsonPropertyName("buyerPhoneNumber")]
        [JsonProperty("buyerPhoneNumber")]
        public required string BuyerPhoneNumber { get; set; }

        [JsonPropertyName("buyerEmail")]
        [JsonProperty("buyerEmail")]
        public required string BuyerEmail { get; set; }

        [JsonPropertyName("buyerCountry")]
        [JsonProperty("buyerCountry")]
        public required string BuyerCountry { get; set; }

        [JsonPropertyName("buyerCity")]
        [JsonProperty("buyerCity")]
        public required string BuyerCity { get; set; }

        [JsonPropertyName("buyerZipCode")]
        [JsonProperty("buyerZipCode")]
        public required string BuyerZipCode { get; set; }

        [JsonPropertyName("buyerAddress")]
        [JsonProperty("buyerAddress")]
        public required string BuyerAddress { get; set; }
    }
}
