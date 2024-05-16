using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace order_managment.Model
{
    public class Order
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonPropertyName("orderId")]
        [JsonProperty("orderId")]
        public string OrderId { get { return Id; } }

        [JsonPropertyName("orderStatus")]
        [JsonProperty("orderStatus")]
        public required string Status { get; set; }

        [JsonPropertyName("orderDescription")]
        [JsonProperty("orderDescription")]
        public string? Description { get; set; }

        [JsonProperty("createdAt")]
        [JsonPropertyName("createdAt")]
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("orderPrice")]
        [JsonProperty("orderPrice")]
        public decimal Price { get; set; }

        [JsonPropertyName("buyer")]
        [JsonProperty("buyer")]
        public Buyer Buyer { get; set; }

        [JsonPropertyName("orderItems")]
        [JsonProperty("orderItems")]
        public required List<OrderItem> OrderItems { get; set; }

        public Order()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
