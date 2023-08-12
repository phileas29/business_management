using FM.Domain.Abstractions.Repository;
using FM.Domain.Models.Repository;
using FM.Repository.Context;

namespace FM.Repository.Services
{
    public class ClientRepository : IClientRepository
    {
        private readonly FirmContext _context;

        public ClientRepository(FirmContext context)
        {
            _context = context;
        }
        public async Task InsertClientAsync(FClient fClient)
        {
            _context.Add(fClient);
            await _context.SaveChangesAsync();
        }
    }
}
