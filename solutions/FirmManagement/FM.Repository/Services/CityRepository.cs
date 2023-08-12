using FM.Repository.Context;
using FM.Domain.Models.Repository;
using FM.Domain.Abstractions.Repository;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace FM.Repository.Services
{
    public class CityRepository : ICityRepository
    {
        private readonly FirmContext _context;

        private static List<CityJsonRepositoryModel>? _Cities { get; set; } = null;

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
        public async Task<int> InsertCityAsync(FCity fCity)
        {
            _context.Add(fCity);
            int res = await _context.SaveChangesAsync();
            return res;
        }

        public async Task<FCity?> SelectCityByCodeAsync(int code)
        {
            FCity? fCity = await _context.FCities.Where(c=>c.CiInseeCode==code).FirstOrDefaultAsync();
            return fCity;
        }

        public CityJsonRepositoryModel? SelectCityByNameFromFranceJsonDb(string cityName)
        {
            _Cities ??= LoadCitiesFromJson();

            return _Cities?
                .Where(city => cityName.Equals(city.Nom, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public async Task<int> UpdateCityAsync(FCity fCity)
        {
            _context.Update(fCity);
            return await _context.SaveChangesAsync();
        }

        public List<CityJsonRepositoryModel> SelectMatchingCitiesFromFranceJsonDb(string input)
        {
            _Cities ??= LoadCitiesFromJson();

            List<CityJsonRepositoryModel> matchingCities = _Cities!
                .Where(city => city.Nom != null && city.Nom.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                .Take(10) // Limit the number of suggestions
                .ToList();

            return matchingCities;
        }
    }
}
