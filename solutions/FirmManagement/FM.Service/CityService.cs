using FM.Domain.Abstractions.Repository;
using FM.Domain.Abstractions.Service;
using FM.Domain.Models.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task<SelectList> GetSelectListAsync()
        {
            List<FCity> fCities = await _cityRepository.SelectAllCitiesAsync();
            List<SelectListItem> citiesWithNull = fCities
                .Select(c => new SelectListItem
                {
                    Text = c.CiName,
                    Value = c.CiId.ToString()
                })
                .ToList();
            citiesWithNull.Insert(0,new SelectListItem { Text = "Select a city", Value = "", Selected = true });
            return new SelectList(citiesWithNull, "Value", "Text", "");
        }
    }
}
