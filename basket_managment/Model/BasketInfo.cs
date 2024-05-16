using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace basket_managment.Model
{
    public class BasketInfo
    {
        [JsonProperty("products")]
        [JsonPropertyName("products")]
        public required List<Product> Products { get; set; } = new List<Product>();
    }
}
