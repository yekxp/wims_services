using inventory_managment.Model;
using inventory_managment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventory_managment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/addCategory")]
        public IActionResult AddCategory([FromBody] Category category)
        {
            _categoryService.AddCategory(category);

            return Ok(category);
        }

        [Authorize(Roles = "Customer, Admin")]
        [HttpGet("/getAllCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategorie()
        {
            var result = await _categoryService.GetAllCategories();

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/getCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var result = await _categoryService.GetCategoryById(id);

            return result is null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("/deleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _categoryService.DeleteCategory(id);

            return Ok();
        }
    }
}
