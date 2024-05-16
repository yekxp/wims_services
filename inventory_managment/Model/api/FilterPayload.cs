namespace inventory_managment.Model.api
{
    public class FilterPayload
    {
        public List<string> WarehouseId { get; set; }
        public List<string> CategoryId { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
    }
}
