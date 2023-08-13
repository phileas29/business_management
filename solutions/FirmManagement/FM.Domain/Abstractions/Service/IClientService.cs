using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;

namespace FM.Domain.Abstractions.Service
{
    public interface IClientService
    {
        Task<FClient> GetRepositoryClientFromWebModelAsync(ClientCreateWebModel wClient);
        Task<int> PutClientAsync(FClient fClient);
        Task<List<FClient>> GetAllClientsAsync();
        Task<ClientIndexWebModel> GetClientIndexWebModel(ClientIndexWebModel wClient);

        //List<FClient> GetClientsByPage(List<FClient> selectedClients, int pageSize, int page);
        //List<FClient>? GetClientsGPS(List<FClient> selectedClients, List<FClient> excludedClients, string? nameSearch, string? firstnameSearch, int? citySearch);
    }
}
