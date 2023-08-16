using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface IInvoiceRepository
    {
        public Task<List<int>> SelectYearsFromInvoicesAsync();
        public int MaxInvoiceId();
        public Task<int> InsertInvoiceAsync(FInvoice fInvoice);
    }
}
