using FM.Domain.Abstractions.Repository;
using FM.Domain.Models.Repository;

namespace FM.Service
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }
        public List<CityJsonRepositoryModel> GetMatchingCitiesFromFranceJsonDb(string input)
        {
            return _cityRepository.SelectMatchingCitiesFromFranceJsonDb(input);
        }
    }
}
