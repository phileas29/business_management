using System.Collections.Generic;
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
    }
}
