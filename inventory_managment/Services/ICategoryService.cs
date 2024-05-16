using inventory_managment.Model;

namespace inventory_managment.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories();

        Task<Category?> GetCategoryById(string id);

        void AddCategory(Category category);

        Task DeleteCategory(string id);
    }
}
