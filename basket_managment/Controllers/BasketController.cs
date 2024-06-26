using basket_managment.Model;
using basket_managment.Services;
using JwtManagerHandler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace basket_managment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("/addToBasket")]
        public async Task<IActionResult> AddToBasket([FromBody] BasketInfo product)
        {
            await _basketService.AddToBasket(product);

            return Ok();
        }

        [Authorize(Roles = "Customer")]
        [HttpDelete("/removeFromBasket/{productId}")]
        public async Task<IActionResult> RemoveFromBasket(string productId)
        {
            string? currentCache = await _basketService.RemoveFromBasket(productId, "BASKET");

            return currentCache == string.Empty ? NotFound() : Ok();
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("/getBasket")]
        public async Task<ActionResult<BasketInfo>> GetBasket()
        {
            var products = await _basketService.GetBasket("BASKET");
            
            return products is null ? NotFound() : Ok(products);
        }

    }
}