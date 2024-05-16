namespace inventory_managment.Model.api
{
    public class RequestProducts
    {
        public required string WarehouseId { get; set; }
        public List<Product>? Products { get; set; }
    }
}
