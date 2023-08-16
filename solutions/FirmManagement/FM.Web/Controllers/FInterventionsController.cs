using FM.Domain.Abstractions.Service;
using FM.Domain.Abstractions.Web;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc;

namespace FM.Web.Controllers
{
    public class FInterventionsController : Controller, IInterventionWeb
    {
        private readonly IInterventionService _interventionService;
        public FInterventionsController(IInterventionService interventionService)
        {
            _interventionService = interventionService;
        }

        // GET: FInterventions/Create/id
        public async Task<IActionResult> CreateAsync(int id)
        {
            InterventionCreateWebModel wIntervention = await _interventionService.SetInterventionWebModelAsync(null, id);
            return View(wIntervention);
        }

        // POST: FInterventions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("Activities, Choice, Client, ClientId, CategoryId, Date, Description, NbRoundTrip")] InterventionCreateWebModel wIntervention)
        {
            if (ModelState.IsValid)
            {
                FIntervention fIntervention = _interventionService.GetRepositoryInterventionFromWebModel(wIntervention);
                int iid = await _interventionService.PutInterventionServiceAsync(fIntervention);
                if (wIntervention.Choice == 1)
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Create", "FInvoices", new { id = wIntervention.ClientId, iid = iid });
            }
            wIntervention = await _interventionService.SetInterventionWebModelAsync(wIntervention);
            return View(wIntervention);
        }
    }
}
