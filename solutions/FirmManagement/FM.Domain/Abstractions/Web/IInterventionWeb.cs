using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc;

namespace FM.Domain.Abstractions.Web
{
    public interface IInterventionWeb
    {
        public Task<IActionResult> CreateAsync(int id);
        public Task<IActionResult> CreateAsync(InterventionCreateWebModel wIntervention);
    }
}
