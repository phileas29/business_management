using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;
using System.Numerics;
using static LittleFirmManagement.Models.FClientUtility;
using System.Text.Json;

namespace LittleFirmManagement.Controllers
{
    public class FClientsController : Controller
    {
        private readonly FirmContext _context;

        public FClientsController(FirmContext context)
        {
            _context = context;
        }

        // GET: FClients
        public async Task<IActionResult> Index(string nameSearch="", string firstnameSearch = "", string citySearch = "")
        {
            var clientsCtx = _context.FClients.Include(f => f.CFkBirthCity).Include(f => f.CFkCity).Include(f => f.CFkMedia);
            var clients = clientsCtx.AsQueryable();

            if (!string.IsNullOrEmpty(nameSearch))
            {
                clients = clients.Where(c => c.CName.Contains(nameSearch));
            }

            if (!string.IsNullOrEmpty(firstnameSearch))
            {
                clients = clients.Where(c => c.CFirstname.Contains(firstnameSearch));
            }

            if (!string.IsNullOrEmpty(citySearch))
            {
                clients = clients.Where(c => c.CFkCity.CiName.Contains(citySearch));
            }

            ViewData["NameSearch"] = nameSearch;
            ViewData["FirstnameSearch"] = firstnameSearch;
            ViewData["CitySearch"] = citySearch;

            return View(clientsCtx);
        }

        // GET: FClients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fClient = await _context.FClients
                .Include(f => f.CFkBirthCity)
                .Include(f => f.CFkCity)
                .Include(f => f.CFkMedia)
                .FirstOrDefaultAsync(m => m.CId == id);
            if (fClient == null)
            {
                return NotFound();
            }

            return View(fClient);
        }

        // GET: FClients/Create
        public IActionResult Create()
        {
            var citiesWithNull = _context.FCities.ToList();
            citiesWithNull.Insert(0, new FCity { CiId = -1, CiName = "Select a city" });
            var mediasWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "média").ToList();
            mediasWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a media" });

            ViewData["CFkBirthCityId"] = new SelectList(citiesWithNull, "CiId", "CiName");
            ViewData["CFkMediaId"] = new SelectList(mediasWithNull, "CaId", "CaName");

            var viewModel = new FClientsViewModel();
            viewModel.CName = "Default Name";
            viewModel.CFirstname = "Default Firstname";
            viewModel.CEmail = "default@example.com";
            viewModel.CPhoneFixed = "1234567890";
            viewModel.CPhoneCell = "9876543210";
            viewModel.CFkMediaId = mediasWithNull[2].CaId;
            return View(viewModel);

            return View();
        }

        // POST: FClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CFkMediaId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder,Town,BirthCityInput")] FClientsViewModel vClient, bool saveAndExit)
        {
            ModelState.Remove("CFkCity");
            ModelState.Remove("CFkBirthCityId");
            if (vClient.CFkMediaId == -1)
                ModelState.AddModelError("CFkMediaId", "Please select a media.");
            if (vClient.CFkBirthCityId == -1)
                vClient.CFkBirthCityId = null;
            if (ModelState.IsValid)
            {
                FClient fClient = FClientUtility.ValidateClient(vClient, _context).Result;
                _context.Add(fClient);
                await _context.SaveChangesAsync();
                if (saveAndExit)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Create", "FInterventions", new { CId = fClient.CId });
            }
            var citiesWithNull = _context.FCities.ToList();
            citiesWithNull.Insert(0, new FCity { CiId = -1, CiName = "Select a city" });
            var mediasWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "média").ToList();
            mediasWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a media" });

            ViewData["CFkBirthCityId"] = new SelectList(citiesWithNull, "CiId", "CiName");
            ViewData["CFkMediaId"] = new SelectList(mediasWithNull, "CaId", "CaName");
            return View(vClient);
        }

        // GET: FClients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fClient = await _context.FClients.FindAsync(id);
            if (fClient == null)
            {
                return NotFound();
            }
            ViewData["CFkBirthCityId"] = new SelectList(_context.FCities, "CiId", "CiId", fClient.CFkBirthCityId);
            ViewData["CFkCityId"] = new SelectList(_context.FCities, "CiId", "CiId", fClient.CFkCityId);
            ViewData["CFkMediaId"] = new SelectList(_context.FCategories, "CaId", "CaId", fClient.CFkMediaId);
            return View(fClient);
        }

        // POST: FClients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CId,CFkMediaId,CFkCityId,CFkBirthCityId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder")] FClient fClient)
        {
            if (id != fClient.CId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fClient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FClientExists(fClient.CId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CFkBirthCityId"] = new SelectList(_context.FCities, "CiId", "CiId", fClient.CFkBirthCityId);
            ViewData["CFkCityId"] = new SelectList(_context.FCities, "CiId", "CiId", fClient.CFkCityId);
            ViewData["CFkMediaId"] = new SelectList(_context.FCategories, "CaId", "CaId", fClient.CFkMediaId);
            return View(fClient);
        }

        // GET: FClients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fClient = await _context.FClients
                .Include(f => f.CFkBirthCity)
                .Include(f => f.CFkCity)
                .Include(f => f.CFkMedia)
                .FirstOrDefaultAsync(m => m.CId == id);
            if (fClient == null)
            {
                return NotFound();
            }

            return View(fClient);
        }

        // POST: FClients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fClient = await _context.FClients.FindAsync(id);
            _context.FClients.Remove(fClient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FClientExists(int id)
        {
            return _context.FClients.Any(e => e.CId == id);
        }
        public IActionResult GetMatchingCities(string input)
        {
            return Json(FClientUtility.GetMatchingCities(input));
        }

    }
}
