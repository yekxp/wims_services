using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace inventory_managment.Model.message
{
    public class Order
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

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
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("orderPrice")]
        [JsonProperty("orderPrice")]
        public decimal Price { get; set; }

        [JsonPropertyName("buyer")]
        [JsonProperty("buyer")]
        public Buyer Buyer { get; set; }

        [JsonPropertyName("orderItems")]
        [JsonProperty("orderItems")]
        public required List<OrderItem> OrderItems { get; set; }

    }
}
