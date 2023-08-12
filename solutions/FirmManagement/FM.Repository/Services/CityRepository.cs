using FM.Repository.Context;
using FM.Domain.Models.Repository;
using FM.Domain.Abstractions.Repository;
using System.Text.Json;

namespace FM.Repository.Services
{
    public class CityRepository : ICityRepository
    {
        private readonly FirmContext _context;

        private static List<CityJsonRepositoryModel>? Cities { get; set; } = null;

        private static List<CityJsonRepositoryModel>? LoadCitiesFromJson()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Ignore case when deserializing JSON properties
            };
            // Load and deserialize the "towns.json" file into a list of City objects
            var json = File.ReadAllText("towns.json");
            return JsonSerializer.Deserialize<List<CityJsonRepositoryModel>>(json, options);
        }

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
            return _context.FCities.Where(c=>c.CiInseeCode==code).FirstOrDefault();
        }

        public CityJsonRepositoryModel? SelectCityByNameFromFranceJsonDb(string cityName)
        {
            Cities ??= LoadCitiesFromJson();

            return Cities?
                .Where(city => city.Nom.Equals(cityName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public async void UpdateCityAsync(FCity fCity)
        {
            _context.Update(fCity);
            await _context.SaveChangesAsync();
        }
    }
}
