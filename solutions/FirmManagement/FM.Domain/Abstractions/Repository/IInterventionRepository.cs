using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface IInterventionRepository
    {
        public Task<List<FIntervention>> SelectAllInvoicedInterventionsByYearAndByClient(int year, FClient client);
    }
}
