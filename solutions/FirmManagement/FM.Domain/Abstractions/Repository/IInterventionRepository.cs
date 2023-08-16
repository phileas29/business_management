using FM.Domain.Models.Repository;
using FM.Domain.Models.Service;

namespace FM.Domain.Abstractions.Repository
{
    public interface IInterventionRepository
    {
        public Task<List<FIntervention>> SelectAllInvoicedInterventionsByYearAndByClient(int year, FClient client);
        public Task<int> InsertInterventionAsync(FIntervention fIntervention);
        public Task<List<InvoiceCreateServiceModel>> SelectUninvoicedInterventionsByClientAsync(int id);
        public Task<int> UpdateInterventionsInvoiceIdAsync(List<FIntervention> interventions, int id);
        public Task<List<FIntervention>> SelectInterventionsByIdsAsync(List<int> ids);
    }
}
