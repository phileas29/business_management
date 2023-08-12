using System.Reflection;
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

        public SelectList GetSelectList(string categoryType)
        {
            List<SelectListItem> mediasWithNull = _categoryRepository
                .SelectCategoriesByType(categoryType)
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            mediasWithNull.Add(new SelectListItem { Text = "Select a media", Value = "", Selected = true });
            return new SelectList(mediasWithNull, "Value", "Text", "");
        }
    }
}
