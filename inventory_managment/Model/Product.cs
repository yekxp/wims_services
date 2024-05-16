using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace inventory_managment.Model
{
    public class Product
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

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

        [JsonProperty("createdAt")]
        [JsonPropertyName("createdAt")]
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonProperty("categoryId")]
        [JsonPropertyName("categoryId")]
        public string? CategoryId { get; set; }

        public Product()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
