using FM.Domain.Abstractions.Repository;
using FM.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace FM.Repository.Services
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly FirmContext _context;

        public InvoiceRepository(FirmContext context)
        {
            _context = context;
        }
        public async Task<List<int>> SelectYearsFromInvoicesAsync()
        {
            return await _context.FInvoices
                .Select(i => i.InInvoiceDate.Year)
                .Distinct()
                .OrderByDescending(y=>y)
                .ToListAsync();
        }
    }
}
