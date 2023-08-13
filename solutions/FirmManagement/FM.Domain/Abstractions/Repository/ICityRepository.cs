using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface ICityRepository
    {
        public Task<int> InsertCityAsync(FCity fCity);
        public Task<FCity?> SelectCityByCodeAsync(int code);
        public Task<List<FCity>> SelectAllCitiesAsync();
        public Task<int> UpdateCityAsync(FCity fCity);
        public CityJsonRepositoryModel? SelectCityByNameFromFranceJsonDb(string cityName);
        public List<CityJsonRepositoryModel> SelectMatchingCitiesFromFranceJsonDb(string input);
    }
}
