using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface ICategoryRepository
    {
        public Task<List<FCategory>> SelectCategoriesByTypeAsync(string categoryType);
    }
}
