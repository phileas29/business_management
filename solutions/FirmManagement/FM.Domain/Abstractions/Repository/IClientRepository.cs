using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface IClientRepository
    {
        public Task<int> InsertClientAsync(FClient fClient);
        public Task<List<FClient>> SelectAllClientsAsync();
        public Task<int> CountClientsAsync();
        public Task<List<FClient>> SelectClientsByNameOrFirstnameOrCityAsync(string? nameSearch, string? firstnameSearch, int? citySearch, int pageSize, int page);
        public Task<List<FClient>> SelectAllInvoicedClientsByYear(int year);
    }
}
