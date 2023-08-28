using FM.Domain.Abstractions.Repository;
using FM.Domain.Models.Repository;
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

        public async Task<int> InsertInvoiceAsync(FInvoice fInvoice)
        {
            _context.Add(fInvoice);
            await _context.SaveChangesAsync();
            return fInvoice.InId;
        }

        public int MaxInvoiceId()
        {
            return _context.FInvoices
                .Max(i => i.InInvoiceId)
                .GetValueOrDefault(-1);
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
