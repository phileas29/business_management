using FM.Domain.Abstractions.Repository;
using FM.Domain.Abstractions.Service;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;

namespace FM.Service
{
    public class InterventionService : IInterventionService
    {
        private readonly IInterventionRepository _interventionRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ICategoryService _categoryService;
        public InterventionService(IInterventionRepository interventionRepository, IClientRepository clientRepository, ICategoryService categoryService)
        {
            _interventionRepository = interventionRepository;
            _clientRepository = clientRepository;
            _categoryService = categoryService;
        }

        public FIntervention GetRepositoryInterventionFromWebModel(InterventionCreateWebModel wIntervention)
        {
            FIntervention fIntervention = new()
            {
                IDate = wIntervention.Date,
                IDescription = wIntervention.Description,
                IFkCategoryId = wIntervention.CategoryId,
                INbRoundTrip = wIntervention.NbRoundTrip,
                IFkClientId = wIntervention.ClientId,
            };

            return fIntervention;
        }

        public async Task<int> PutInterventionServiceAsync(FIntervention fIntervention)
        {
            return await _interventionRepository.InsertInterventionAsync(fIntervention);
        }

        public async Task<InterventionCreateWebModel> SetInterventionWebModelAsync(InterventionCreateWebModel? wIntervention, int id)
        {
            InterventionCreateWebModel wInterventionResult;
            if (wIntervention == null)
            {
                wInterventionResult = new InterventionCreateWebModel
                {
                    Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    NbRoundTrip = 1,
                    ClientId = id,
                };
            }
            else
            {
                wInterventionResult = new InterventionCreateWebModel
                {
                    Date = wIntervention.Date,
                    Description = wIntervention.Description,
                    CategoryId = wIntervention.CategoryId,
                    NbRoundTrip = wIntervention.NbRoundTrip
                };
            }
            wInterventionResult.Client = await _clientRepository.SelectClientByIdAndIncludeCityAsync(id);
            wInterventionResult.Activities = await _categoryService.GetSelectListAsync("activité");
            return wInterventionResult;
        }
    }
}
