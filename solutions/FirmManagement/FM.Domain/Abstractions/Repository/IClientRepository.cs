using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface IClientRepository
    {
        public Task<int> InsertClientAsync(FClient fClient);
        public Task<List<FClient>> SelectAllClientsAsync();
    }
}
