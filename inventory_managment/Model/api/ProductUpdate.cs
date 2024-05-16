using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace inventory_managment.Model.api
{
    public class ProductUpdate
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonProperty("productName")]
        [JsonPropertyName("productName")]
        public string? Name { get; set; }

        [JsonProperty("productDescription")]
        [JsonPropertyName("productDescription")]
        public string? Description { get; set; }

        [JsonProperty("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; } = -1;

        [JsonProperty("quantity")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; } = -1;

        [JsonProperty("categoryId")]
        [JsonPropertyName("categoryId")]
        public string? CategoryId { get; set; }
    }
}
