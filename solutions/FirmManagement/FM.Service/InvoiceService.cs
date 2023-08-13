using FM.Domain.Abstractions.Repository;
using FM.Domain.Abstractions.Service;
using FM.Domain.Models.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<SelectList> GetYearsSelectListAsync()
        {
            List<int> years = await _invoiceRepository.SelectYearsFromInvoicesAsync();
            List<SelectListItem> yearsWithNull = years
                .Select(y => new SelectListItem
                {
                    Text = y.ToString(),
                    Value = y.ToString()
                })
                .ToList();
            yearsWithNull.Insert(0, new SelectListItem { Text = "Select a year", Value = "", Selected = true });
            return new SelectList(yearsWithNull, "Value", "Text", "");
        }
    }
}
