using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;

namespace FM.Domain.Abstractions.Service
{
    public interface IClientService
    {
        FClient GetRepositoryClientFromWebModel(ClientWebModel wClient);
        Task PutClient(FClient fClient);
    }
}
