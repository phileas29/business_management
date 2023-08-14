using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;

namespace FM.Domain.Abstractions.Service
{
    public interface IClientService
    {
        Task<FClient> GetRepositoryClientFromWebModelAsync(ClientCreateWebModel wClient);
        Task<int> PutClientAsync(FClient fClient);
        Task<List<FClient>> GetAllClientsAsync();
        Task<bool> GenerateTaxCertificates(ClientGenerateTaxCertificatesWebModel wClient, string webRootPath);
        Task<ClientIndexWebModel> GetClientIndexWebModelAsync(ClientIndexWebModel wClient);
    }
}
