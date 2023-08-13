namespace FM.Domain.Abstractions.Repository
{
    public interface IInvoiceRepository
    {
        public Task<List<int>> SelectYearsFromInvoicesAsync();
    }
}
