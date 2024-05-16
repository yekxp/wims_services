using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace order_managment.Model
{
    public class OrdersAnalytics
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonPropertyName("metadataId")]
        [JsonProperty("metadataId")]
        public string MetadataId { get { return Id; } }

        [JsonPropertyName("totalPricePerMonth")]
        [JsonProperty("totalPricePerMonth")]
        public Dictionary<string, decimal> TotalPricePerMonth { get; set; } = new Dictionary<string, decimal>();

        [JsonPropertyName("totalOrdersPerMonth")]
        [JsonProperty("totalOrdersPerMonth")]
        public Dictionary<string, int> TotalOrdersPerMonth { get; set; } = new Dictionary<string, int>();

        [JsonPropertyName("totalPriceOverall")]
        [JsonProperty("totalPriceOverall")]
        public decimal TotalPriceOverall => TotalPricePerMonth.Values.Sum();

        [JsonPropertyName("totalOrdersOverall")]
        [JsonProperty("totalOrdersOverall")]
        public int TotalOrdersOverall => TotalOrdersPerMonth.Values.Sum();

        public void AddOrder(DateTime date, decimal totalPrice)
        {
            string monthYear = $"{date.Month}-{date.Year}";

            if (TotalPricePerMonth.ContainsKey(monthYear))
            {
                TotalPricePerMonth[monthYear] += totalPrice;
                TotalOrdersPerMonth[monthYear]++;
            }
            else
            {
                TotalPricePerMonth.Add(monthYear, totalPrice);
                TotalOrdersPerMonth.Add(monthYear, 1);
            }
        }

        public OrdersAnalytics()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
