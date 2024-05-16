namespace inventory_managment.Model.api
{
    public class RequestProduct
    {
        public required string WarehouseId { get; set; }
        public ProductUpdate Product { get; set; }
    }
}
