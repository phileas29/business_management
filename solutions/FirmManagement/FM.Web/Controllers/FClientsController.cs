using FM.Domain.Abstractions.Service;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc;

namespace LittleFirmManagement.Controllers
{
    public class FClientsController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IClientService _clientService;
        public FClientsController(ICategoryService categoryService, IClientService clientService)
        {
            _categoryService = categoryService;
            _clientService = clientService;
        }

        // GET: FClients/Create
        public IActionResult Create()
        {
            ClientWebModel model = new();
            model.Medias = _categoryService.GetSelectList("m�dia");
            return View(model);
        }

        // POST: FClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CFkMediaId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder,Town,BirthCityInput")] ClientWebModel model, bool saveAndExit)
        {
            if (ModelState.IsValid)
            {
                FClient fClient = _clientService.GetRepositoryClientFromWebModel(model);
                await _clientService.PutClient(fClient);
                if (saveAndExit)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Create", "FInterventions", new { id = fClient.CId });
            }
            model.Medias = _categoryService.GetSelectList("m�dia");
            return View(model);
        }
    }

}
