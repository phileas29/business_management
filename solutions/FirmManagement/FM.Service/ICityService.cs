using FM.Domain.Models.Repository;

namespace FM.Service
{
    public interface ICityService
    {
        public List<CityJsonRepositoryModel> GetMatchingCitiesFromFranceJsonDb(string input);

    }
}