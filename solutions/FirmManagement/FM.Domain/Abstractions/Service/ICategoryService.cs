using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Abstractions.Service
{
    public interface ICategoryService
    {
        public Task<SelectList> GetSelectListAsync(string categoryType);
    }
}
