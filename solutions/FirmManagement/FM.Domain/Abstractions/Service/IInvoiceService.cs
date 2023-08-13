using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Abstractions.Service
{
    public interface IInvoiceService
    {
        public Task<SelectList> GetYearsSelectListAsync();
    }
}
