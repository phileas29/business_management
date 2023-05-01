using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;

namespace LittleFirmManagement.Controllers
{
    public class FCitiesController : Controller
    {
        private readonly FirmContext _context;

        public FCitiesController(FirmContext context)
        {
            _context = context;
        }

        // GET: FCities
        public async Task<IActionResult> Index()
        {
            return View(await _context.FCities.ToListAsync());
        }

        // GET: FCities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fCity = await _context.FCities
                .FirstOrDefaultAsync(m => m.CiId == id);
            if (fCity == null)
            {
                return NotFound();
            }

            return View(fCity);
        }

        // GET: FCities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FCities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CiId,CiPostalCode,CiName,CiInseeCode,CiDepartCode")] FCity fCity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fCity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fCity);
        }

        // GET: FCities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fCity = await _context.FCities.FindAsync(id);
            if (fCity == null)
            {
                return NotFound();
            }
            return View(fCity);
        }

        // POST: FCities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CiId,CiPostalCode,CiName,CiInseeCode,CiDepartCode")] FCity fCity)
        {
            if (id != fCity.CiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fCity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FCityExists(fCity.CiId))
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
            return View(fCity);
        }

        // GET: FCities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fCity = await _context.FCities
                .FirstOrDefaultAsync(m => m.CiId == id);
            if (fCity == null)
            {
                return NotFound();
            }

            return View(fCity);
        }

        // POST: FCities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fCity = await _context.FCities.FindAsync(id);
            _context.FCities.Remove(fCity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FCityExists(int id)
        {
            return _context.FCities.Any(e => e.CiId == id);
        }
    }
}
