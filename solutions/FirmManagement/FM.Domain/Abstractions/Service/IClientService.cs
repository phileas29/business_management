using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;

namespace FM.Domain.Abstractions.Service
{
    public interface IClientService
    {
        Task<FClient> GetRepositoryClientFromWebModelAsync(ClientWebModel wClient);
        Task<int> PutClientAsync(FClient fClient);
        Task<List<FClient>> GetAllClientsAsync();

    }
}
