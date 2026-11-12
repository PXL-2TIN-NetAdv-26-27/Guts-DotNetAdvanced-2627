namespace BethanysPieShop.Models
{
    public class CategoryDbRepository: ICategoryRepository
    {
        private readonly BethanysPieShopDbContext _context;

        public CategoryDbRepository(BethanysPieShopDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> AllCategories => _context.Categories.OrderBy(p => p.CategoryName);
    }
}
