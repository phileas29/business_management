using FM.Domain.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace LittleFirmManagement.Controllers
{
    public class FClientsController : Controller
    {
        public FClientsController()
        {
        }

        // GET: FClients/Create
        public IActionResult Create()
        {
            ClientWebModel model = new();

            SetClient(model);
            return View(model);
        }

        // POST: FClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CFkMediaId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder,Town,BirthCityInput")] FClientCreateViewModel model, bool saveAndExit)
        {
            if (ModelState.IsValid)
            {
                FClient fClient = GetRepositoryClientFromWebModel(model);
                PutClient(ClientRepositoryModel);
                if (saveAndExit)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Create", "FInterventions", new { id = fClient.CId });
            }
            SetClient(model);
            return View(model);
        }
    }

}
