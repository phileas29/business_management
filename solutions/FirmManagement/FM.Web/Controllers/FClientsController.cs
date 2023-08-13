using FM.Domain.Abstractions.Service;
using FM.Domain.Abstractions.Web;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;
using FM.Service;
using Microsoft.AspNetCore.Mvc;

namespace LittleFirmManagement.Controllers
{
    public class FClientsController : Controller, IClientWeb
    {
        private readonly ICategoryService _categoryService;
        private readonly IClientService _clientService;
        private readonly ICityService _cityService;
        public FClientsController(ICategoryService categoryService, IClientService clientService, ICityService cityService)
        {
            _categoryService = categoryService;
            _clientService = clientService;
            _cityService = cityService;
        }

        // GET: FClients/Create
        public async Task<IActionResult> CreateAsync()
        {
            ClientCreateWebModel model = new();
            model.Medias = await _categoryService.GetSelectListAsync("média");
            return View(model);
        }

        // POST: FClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("CFkMediaId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,IsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder,Town,BirthCityInput,EnableUrssafPayment,Choice")] ClientCreateWebModel model)
        {
            if (ModelState.IsValid)
            {
                FClient fClient = await _clientService.GetRepositoryClientFromWebModelAsync(model);
                await _clientService.PutClientAsync(fClient);
                if (model.Choice == 1)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Create", "FInterventions", new { id = fClient.CId });
            }
            model.Medias = await _categoryService.GetSelectListAsync("média");
            return View(model);
        }
        public IActionResult GetMatchingCities(string input)
        {
            return Json(_cityService.GetMatchingCitiesFromFranceJsonDb(input));
        }

        // GET: FClients/Index
        public async Task<IActionResult> IndexAsync(ClientIndexWebModel wClient)
        {
            return View(await _clientService.GetClientIndexWebModel(wClient));
        }
    }

}
