using FM.Domain.Abstractions.Repository;
using FM.Domain.Abstractions.Service;
using FM.Domain.Models.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<SelectList> GetSelectListAsync(string categoryType)
        {
            List<FCategory> fCategories = await _categoryRepository
                .SelectCategoriesByTypeAsync(categoryType);
            List<SelectListItem> mediasWithNull = fCategories
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            mediasWithNull.Insert(0,new SelectListItem { Text = "Select a " + categoryType, Value = "", Selected = true });
            return new SelectList(mediasWithNull, "Value", "Text", "");
        }
    }
}
