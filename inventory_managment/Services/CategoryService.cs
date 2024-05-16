using inventory_managment.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Reflection.Metadata;

namespace inventory_managment.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly Container _container;
        public CategoryService(ILogger<CategoryService> logger, CosmosClient cosmosClient, IConfiguration configuration)
        {
            _logger = logger;
            Database database = cosmosClient.GetDatabase(configuration["CosmosDbSettings:DatabaseName"]);
            _container = database.CreateContainerIfNotExistsAsync("IM_Category", "/categoryId").Result;
        }

        public void AddCategory(Category category)
        {
            _container.CreateItemAsync(category).Wait();
            _logger.LogDebug("Sucessfully created category");
        }

        public async Task DeleteCategory(string id)
        {
            Category? category = await GetCategoryById(id);

            if (category is not null)
            {
                await _container.DeleteItemAsync<Category>(category.Id, new PartitionKey("63cdb2ca-a19e-44c9-91e7-495e1468ce01"));
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var items = _container.GetItemLinqQueryable<Category>().ToFeedIterator();

            List<Category> categories = new List<Category>();

            if (items.HasMoreResults)
            {
                var result = await items.ReadNextAsync();
                categories.AddRange(result);
            }

            return categories;
        }

        public async Task<Category?> GetCategoryById(string id)
        {
            var query = _container.GetItemLinqQueryable<Category>()
                .Where(c => c.Id == id)
                .Take(1)
                .ToFeedIterator();

            var category = await query.ReadNextAsync();

            return category.FirstOrDefault();
        }
    }
}
