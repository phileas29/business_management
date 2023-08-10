using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface IClientRepository
    {
        public void InsertClientAsync(FClient fClient);
    }
}
