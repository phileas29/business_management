using FM.Domain.Abstractions.Repository;
using FM.Domain.Models.Repository;
using FM.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace FM.Repository.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FirmContext _context;

        public CategoryRepository(FirmContext context)
        {
            _context = context;
        }
        public async Task<List<FCategory>> SelectCategoriesByTypeAsync(string categoryType)
        {
            return await _context.FCategories
                .Where(c=>c.CaFkCategoryType.CtName.Equals(categoryType))
                .ToListAsync();
        }
    }
}
