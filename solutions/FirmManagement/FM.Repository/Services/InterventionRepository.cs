using FM.Domain.Abstractions.Repository;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Service;
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
            await _context.SaveChangesAsync();
            return fIntervention.IId;
        }

        public async Task<List<FIntervention>> SelectAllInvoicedInterventionsByYearAndByClient(int year, FClient client)
        {
            return await _context.FInterventions
                .Include(i => i.IFkInvoice)
                .Include(i => i.IFkCategory)
                .Where(i => i.IFkClient == client && i.IFkInvoice != null && i.IFkInvoice.InInvoiceDate.Year == year)
                .ToListAsync();
        }

        public async Task<List<FIntervention>> SelectInterventionsByIdsAsync(List<int> ids)
        {
            List<FIntervention> fInterventions = await _context.FInterventions
                .Where(i => ids.Contains(i.IId))
                .ToListAsync();
            return fInterventions;
        }

        public Task<List<InvoiceCreateServiceModel>> SelectUninvoicedInterventionsByClientAsync(int id)
        {
            return _context.FInterventions
                .Where(i => i.IFkClientId == id && i.IFkInvoiceId == null)
                .OrderByDescending(i => i.IDate)
                .Select(c => new InvoiceCreateServiceModel()
                {
                    Id = c.IId,
                    Date = c.IDate,
                    Description = c.IDescription
                })
                .ToListAsync();
        }

        public async Task<int> UpdateInterventionsInvoiceIdAsync(List<FIntervention> fInterventions, int id)
        {
            foreach (FIntervention inter in fInterventions)
                inter.IFkInvoiceId = id;
            _context.UpdateRange(fInterventions);
            int res = await _context.SaveChangesAsync();
            return res;
        }
    }
}
