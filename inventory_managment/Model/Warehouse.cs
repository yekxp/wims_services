using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace inventory_managment.Model
{
    public class Warehouse
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonProperty("warehouseId")]
        [JsonPropertyName("warehouseId")]
        public string WarehouseId { get { return Id; } }

        [JsonProperty("warehouseName")]
        [JsonPropertyName("warehouseName")]
        public required string Name { get; set; }

        [JsonProperty("warehouseCountry")]
        [JsonPropertyName("warehouseCountry")]
        public required string Country { get; set; }

        [JsonProperty("warehouseCity")]
        [JsonPropertyName("warehouseCity")]
        public required string City { get; set; }

        [JsonProperty("warehousePostalCode")]
        [JsonPropertyName("warehousePostalCode")]
        public required string PostalCode { get; set; }

        [JsonProperty("warehouseProducts")]
        [JsonPropertyName("warehouseProducts")]
        public List<Product> Products { get; set; } = new List<Product>();

        public Warehouse()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
