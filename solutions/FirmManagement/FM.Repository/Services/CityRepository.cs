using FM.Repository.Context;
using FM.Domain.Models.Repository;
using FM.Domain.Abstractions.Repository;

namespace FM.Repository.Services
{
    public class CityRepository : ICityRepository
    {
        private readonly FirmContext _context;

        public CityRepository(FirmContext context)
        {
            _context = context;
        }
        public async void InsertCityAsync(FCity fCity)
        {
            _context.Add(fCity);
            await _context.SaveChangesAsync();
        }

        public FCity SelectCityByCode(int code)
        {
            throw new NotImplementedException();
        }

        public CityJsonRepositoryModel SelectCityByNameFromFranceJsonDb(string cityName)
        {
            throw new NotImplementedException();
        }

        public void UpdateCity(FCity fCity)
        {
            throw new NotImplementedException();
        }
    }
}
