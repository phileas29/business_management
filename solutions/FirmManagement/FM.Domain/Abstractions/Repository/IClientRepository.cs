using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface IClientRepository
    {
        public Task InsertClientAsync(FClient fClient);
        public List<FClient> SelectAllClients();
    }
}
