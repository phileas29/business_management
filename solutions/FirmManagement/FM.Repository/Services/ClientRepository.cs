using FM.Domain.Abstractions.Repository;
using FM.Domain.Models.Repository;
using FM.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace FM.Repository.Services
{
    public class ClientRepository : IClientRepository
    {
        private readonly FirmContext _context;

        public ClientRepository(FirmContext context)
        {
            _context = context;
        }

        public async Task<int> CountClientsAsync()
        {
            return await _context.FClients.CountAsync();
        }

        public async Task<int> InsertClientAsync(FClient fClient)
        {
            _context.Add(fClient);
            int res = await _context.SaveChangesAsync();
            return res;
        }
        public async Task<List<FClient>> SelectAllClientsAsync()
        {
            List<FClient> res = await _context.FClients.ToListAsync();
            return res;
        }

        public async Task<List<FClient>> SelectAllInvoicedClientsByYear(int year)
        {
            return await _context.FInterventions
                .Include(i => i.IFkClient.CFkCity)
                .Where(i => i.IFkInvoice != null && i.IFkInvoice.InInvoiceDate.Year == year)
                .Select(i => i.IFkClient)
                .Distinct()
                .ToListAsync();
        }

        public async Task<FClient> SelectClientByIdAndIncludeCityAsync(int id)
        {
            FClient client = await _context.FClients
                .Include(c => c.CFkCity)
                .FirstAsync(c => c.CId == id);
            return client;
        }

        public async Task<List<FClient>> SelectClientsByNameOrFirstnameOrCityAsync(string? nameSearch, string? firstnameSearch, int? citySearch, int pageSize, int page)
        {
            var clients = _context.FClients
                .OrderBy(c => c.CName)
                .Include(f => f.CFkCity)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nameSearch))
                clients = clients.Where(c => c.CName.ToLower().Contains(nameSearch.ToLower()));
            //clients = clients.Where(c => -1 < c.CName.IndexOf(model.NameSearch, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(firstnameSearch))
                clients = clients.Where(c => c.CFirstname.ToLower().Contains(firstnameSearch.ToLower()));

            if (0 < citySearch)
                clients = clients.Where(c => c.CFkCity.CiId == citySearch);

            return await clients.ToListAsync();
        }
    }
}
