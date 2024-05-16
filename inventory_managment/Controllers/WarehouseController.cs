using inventory_managment.Model;
using inventory_managment.Model.api;
using inventory_managment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventory_managment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [Authorize]
        [HttpPost("addWarehouse")]
        public async Task<IActionResult> AddWarehouse([FromBody] Warehouse warehouse)
        {
            await _warehouseService.AddWarehouse(warehouse);

            return Ok();
        }

        [Authorize]
        [HttpGet("getAllWarehouses")]
        public async Task<ActionResult<List<Warehouse>>> GetAllWarehouses()
        {
            var warehouses = await _warehouseService.GetAllWarehouses();
            return Ok(warehouses);
        }

        [Authorize]
        [HttpGet("getWarehouseById/{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseById(string id)
        {
            var warehouse = await _warehouseService.GetWarehouseById(id);
            return Ok(warehouse);
        }

        [Authorize]
        [HttpPost("filter")]
        public async Task<ActionResult<List<Product>>> GetWarehouseById([FromBody] FilterPayload filterPayload)
        {
            var products = await _warehouseService.FilterProducts(filterPayload);
            return Ok(products);
        }

        [Authorize]
        [HttpPost("addProducts")]
        public async Task<IActionResult> AddProductsToWarehouse([FromBody] RequestProducts requestProducts)
        {
            await _warehouseService.AddProductsToWarehouse(requestProducts);
            return Ok();
        }

        [Authorize]
        [HttpPut("updateProductInfo")]
        public async Task<IActionResult> UpdateProductInfo([FromBody] RequestProduct requestProduct)
        {
            await _warehouseService.UpdateProductInfo(requestProduct);
            return Ok();
        }

        [Authorize]
        [HttpDelete("deleteProductFromWarehouse")]
        public async Task<IActionResult> DeleteProductFromWarehouse([FromBody] RequestProducts requestProducts)
        {
            await _warehouseService.RemoveProductsFromWareHouse(requestProducts);
            return Ok();
        }
    }
}
