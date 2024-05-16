using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace basket_managment.Model
{
    public class Product
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("productName")]
        [JsonPropertyName("productName")]
        public required string Name { get; set; }

        [JsonProperty("productDescription")]
        [JsonPropertyName("productDescription")]
        public required string Description { get; set; }

        [JsonProperty("price")]
        [JsonPropertyName("price")]
        public required decimal Price { get; set; }

        [JsonProperty("quantity")]
        [JsonPropertyName("quantity")]
        public required int Quantity { get; set; }
    }
}
