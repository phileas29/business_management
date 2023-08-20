using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;

namespace FM.Domain.Abstractions.Service
{
    public interface IClientService
    {
        public Task<FClient> GetRepositoryClientFromWebModelAsync(ClientCreateWebModel wClient);
        public Task<int> PutClientAsync(FClient fClient);
        public Task<List<FClient>> GetAllClientsAsync();
        public Task<bool> GenerateTaxCertificates(ClientGenerateTaxCertificatesWebModel wClient, string webRootPath);
        public Task<ClientIndexWebModel> GetClientIndexWebModelAsync(ClientIndexWebModel wClient);
    }
}
