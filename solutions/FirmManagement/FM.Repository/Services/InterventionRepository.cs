using FM.Domain.Abstractions.Repository;
using FM.Domain.Models.Repository;
using FM.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace FM.Repository.Services
{
    public class InterventionRepository : IInterventionRepository
    {
        private readonly FirmContext _context;

        public InterventionRepository(FirmContext context)
        {
            _context = context;
        }

        public async Task<int> InsertInterventionAsync(FIntervention fIntervention)
        {
            _context.Add(fIntervention);
            int res = await _context.SaveChangesAsync();
            return res;
        }

        public async Task<List<FIntervention>> SelectAllInvoicedInterventionsByYearAndByClient(int year, FClient client)
        {
            return await _context.FInterventions
                .Include(i => i.IFkInvoice)
                .Include(i => i.IFkCategory)
                .Where(i => i.IFkClient == client && i.IFkInvoice != null && i.IFkInvoice.InInvoiceDate.Year == year)
                .ToListAsync();
        }
    }
}
