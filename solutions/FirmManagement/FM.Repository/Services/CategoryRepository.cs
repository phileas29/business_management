using FM.Domain.Abstractions.Repository;
using FM.Domain.Models.Repository;
using FM.Repository.Context;

namespace FM.Repository.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FirmContext _context;

        public CategoryRepository(FirmContext context)
        {
            _context = context;
        }
        public List<FCategory> SelectCategoriesByType(string categoryType)
        {
            return _context.FCategories
                .Where(c=>c.CaFkCategoryType.CtName.Equals(categoryType))
                .ToList();
        }
    }
}
