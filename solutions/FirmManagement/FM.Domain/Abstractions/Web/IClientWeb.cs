using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc;

namespace FM.Domain.Abstractions.Web
{
    public interface IClientWeb
    {
        public Task<IActionResult> CreateAsync();
        public Task<IActionResult> CreateAsync(ClientCreateWebModel wClient);
        public IActionResult GetMatchingCities(string input);
        public Task<IActionResult> IndexAsync(ClientIndexWebModel wClient);
        public Task<IActionResult> GenerateTaxCertificatesAsync();
    }
}
