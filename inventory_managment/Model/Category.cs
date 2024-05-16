using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace inventory_managment.Model
{
    public class Category
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonPropertyName("categoryId")]
        [JsonProperty("categoryId")]
        public string CategoryId { get { return Id; } }

        [JsonPropertyName("categoryName")]
        [JsonProperty("categoryName")]
        public required string Name { get; set; }

        public Category()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
