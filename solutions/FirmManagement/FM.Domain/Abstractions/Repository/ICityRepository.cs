using FM.Domain.Models.Repository;

namespace FM.Domain.Abstractions.Repository
{
    public interface ICityRepository
    {
        public void InsertCityAsync(FCity fCity);
        public FCity SelectCityByCode(int code);
        public void UpdateCity(FCity fCity);
        public CityJsonRepositoryModel SelectCityByNameFromFranceJsonDb(string cityName);
    }
}
