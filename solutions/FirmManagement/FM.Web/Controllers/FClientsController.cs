using FM.Domain.Abstractions.Service;
using FM.Domain.Abstractions.Web;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc;

namespace LittleFirmManagement.Controllers
{
    public class FClientsController : Controller, IClientWeb
    {
        private readonly ICategoryService _categoryService;
        private readonly IClientService _clientService;
        private readonly ICityService _cityService;
        private readonly IInvoiceService _invoiceService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FClientsController(ICategoryService categoryService, IClientService clientService, ICityService cityService, IInvoiceService invoiceService, IWebHostEnvironment webHostEnvironment)
        {
            _categoryService = categoryService;
            _clientService = clientService;
            _cityService = cityService;
            _invoiceService = invoiceService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: FClients/Create
        public async Task<IActionResult> CreateAsync()
        {
            ClientCreateWebModel wClient = new();
            wClient.Medias = await _categoryService.GetSelectListAsync("média");
            return View(wClient);
        }

        // POST: FClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("CFkMediaId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,IsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder,Town,BirthCityInput,EnableUrssafPayment,Choice")] ClientCreateWebModel wClient)
        {
            if (ModelState.IsValid)
            {
                FClient fClient = await _clientService.GetRepositoryClientFromWebModelAsync(wClient);
                await _clientService.PutClientAsync(fClient);
                if (wClient.Choice == 1)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Create", "FInterventions", new { id = fClient.CId });
            }
            wClient.Medias = await _categoryService.GetSelectListAsync("média");
            return View(wClient);
        }

        public async Task<IActionResult> GenerateTaxCertificatesAsync()
        {
            ClientGenerateTaxCertificatesWebModel wClient = new()
            {
                CivilYear = await _invoiceService.GetYearsSelectListAsync()
            };
            return View(wClient);
        }

        // POST: FClients/GenerateTaxCertificates
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateTaxCertificatesAsync([Bind("CivilYear,CivilYearId")] ClientGenerateTaxCertificatesWebModel wClient)
        {
            if (ModelState.IsValid)
            {
                await _clientService.GenerateTaxCertificates(wClient, _webHostEnvironment.WebRootPath);
                return RedirectToAction("Index", "Home");
            }
            return View(wClient);
        }

        public IActionResult GetMatchingCities(string input)
        {
            return Json(_cityService.GetMatchingCitiesFromFranceJsonDb(input));
        }

        // GET: FClients/Index
        public async Task<IActionResult> IndexAsync(ClientIndexWebModel wClient)
        {
            return View(await _clientService.GetClientIndexWebModelAsync(wClient));
        }
    }
}
