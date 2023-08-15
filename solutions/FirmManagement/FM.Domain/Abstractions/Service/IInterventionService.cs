using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;

namespace FM.Domain.Abstractions.Service
{
    public interface IInterventionService
    {
        public Task<int> PutInterventionServiceAsync(FIntervention fIntervention);
        public FIntervention GetRepositoryInterventionFromWebModel(InterventionCreateWebModel wIntervention);
        public Task<InterventionCreateWebModel> SetInterventionWebModelAsync(InterventionCreateWebModel? wIntervention, int id);
    }
}
