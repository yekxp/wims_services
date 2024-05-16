using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace order_managment.Model
{
    public class OrderItem
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("productName")]
        [JsonPropertyName("productName")]
        public required string Name { get; set; }

        [JsonProperty("quantity")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonProperty("categoryId")]
        [JsonPropertyName("categoryId")]
        public string? CategoryId { get; set; }
    }
}
