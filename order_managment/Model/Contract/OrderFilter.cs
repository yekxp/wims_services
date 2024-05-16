using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace order_managment.Model.Contract
{
    public class OrderFilter
    {
        [JsonProperty("buyerFullName")]
        [JsonPropertyName("buyerFullName")]
        public string? BuyerFullName { get; set; }

        [JsonProperty("orderPrice")]
        [JsonPropertyName("orderPrice")]
        public string? Price { get; set; }

        [JsonProperty("orderStatus")]
        [JsonPropertyName("orderStatus")]
        public string? Status { get; set; }

        [JsonProperty("sortCreatedAt")]
        [JsonPropertyName("sortCreatedAt")]
        public bool SortCreatedAt { get; set; } = false;

        [JsonProperty("sortPrice")]
        [JsonPropertyName("sortPrice")]
        public bool SortPrice { get; set; } = false;

        [JsonProperty("sortCreatedAtDescending")]
        [JsonPropertyName("sortCreatedAtDescending")]
        public bool SortCreatedAtDescending { get; set; } = false;

        [JsonProperty("sortPriceDescending")]
        [JsonPropertyName("sortPriceDescending")]
        public bool SortPriceDescending { get; set; } = false;
    }
}
