using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;
using System.Numerics;
using static LittleFirmManagement.Models.CityUtility;
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
        public async Task<IActionResult> Index()
        {
            var firmContext = _context.FClients.Include(f => f.CFkBirthCity).Include(f => f.CFkCity).Include(f => f.CFkMedia);
            return View(await firmContext.ToListAsync());
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
            return View();
        }

        // POST: FClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CId,CFkMediaId,CFkCityId,CFkBirthCityId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder")] FClient fClient, string town, string? birthCity = null)
        {
            ModelState.Remove("CFkCity");
            ModelState.Remove("CFkBirthCityId");
            if (fClient.CFkMediaId == -1)
                ModelState.AddModelError("CFkMediaId", "Please select a media.");
            if (fClient.CFkBirthCityId == -1)
                fClient.CFkBirthCityId = null;
            if (ModelState.IsValid)
            {
                foreach (string cityRaw in new List<string>(){ town, birthCity}) {
                    if (CityUtility.ValidateCity(cityRaw, out CityUtility.City city))
                    {
                        FCity cityDb = _context.FCities.Where(c => c.CiInseeCode == Int32.Parse(city.Code)).FirstOrDefault();
                        if (cityDb == null) {
                            _context.Add(new FCity { CiPostalCode = city.CodesPostaux[0], CiName = city.Nom.ToUpper(), CiInseeCode = Int32.Parse(city.Code), CiDepartCode = Int32.Parse(city.CodeDepartement) });
                            await _context.SaveChangesAsync();
                            fClient.CFkCityId = _context.FCities.Where(c => c.CiName.ToLower() == city.Nom.ToLower()).Select(c => c.CiId).FirstOrDefault();
                        } else
                            fClient.CFkCityId = cityDb.CiId;
                        _context.Add(fClient);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var citiesWithNull = _context.FCities.ToList();
            citiesWithNull.Insert(0, new FCity { CiId = -1, CiName = "Select a city" });
            var mediasWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "média").ToList();
            mediasWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a media" });

            ViewData["CFkBirthCityId"] = new SelectList(citiesWithNull, "CiId", "CiName");
            ViewData["CFkMediaId"] = new SelectList(mediasWithNull, "CaId", "CaName");
            return View(fClient);
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
            return Json(CityUtility.GetMatchingCities(input));
        }

    }
}
