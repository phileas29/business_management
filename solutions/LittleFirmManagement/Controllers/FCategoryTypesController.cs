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
    public class FCategoryTypesController : Controller
    {
        private readonly FirmContext _context;

        public FCategoryTypesController(FirmContext context)
        {
            _context = context;
        }

        // GET: FCategoryTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.FCategoryTypes.ToListAsync());
        }

        // GET: FCategoryTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fCategoryType = await _context.FCategoryTypes
                .FirstOrDefaultAsync(m => m.CtId == id);
            if (fCategoryType == null)
            {
                return NotFound();
            }

            return View(fCategoryType);
        }

        // GET: FCategoryTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FCategoryTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CtId,CtName")] FCategoryType fCategoryType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fCategoryType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fCategoryType);
        }

        // GET: FCategoryTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fCategoryType = await _context.FCategoryTypes.FindAsync(id);
            if (fCategoryType == null)
            {
                return NotFound();
            }
            return View(fCategoryType);
        }

        // POST: FCategoryTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CtId,CtName")] FCategoryType fCategoryType)
        {
            if (id != fCategoryType.CtId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fCategoryType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FCategoryTypeExists(fCategoryType.CtId))
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
            return View(fCategoryType);
        }

        // GET: FCategoryTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fCategoryType = await _context.FCategoryTypes
                .FirstOrDefaultAsync(m => m.CtId == id);
            if (fCategoryType == null)
            {
                return NotFound();
            }

            return View(fCategoryType);
        }

        // POST: FCategoryTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fCategoryType = await _context.FCategoryTypes.FindAsync(id);
            _context.FCategoryTypes.Remove(fCategoryType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FCategoryTypeExists(int id)
        {
            return _context.FCategoryTypes.Any(e => e.CtId == id);
        }
    }
}
