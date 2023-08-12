using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface ICategoryRepository
    {
        List<FCategory> SelectCategoriesByType(string categoryType);
    }
}
