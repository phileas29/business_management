using FM.Domain.Models.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Abstractions.Service
{
    public interface ICityService
    {
        public List<CityJsonRepositoryModel> GetMatchingCitiesFromFranceJsonDb(string input);
        Task<SelectList> GetSelectListAsync();
    }
}