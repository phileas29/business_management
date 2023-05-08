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
    public class FCategoriesController : Controller
    {
        private readonly FirmContext _context;

        public FCategoriesController(FirmContext context)
        {
            _context = context;
        }

        // GET: FCategories
        public async Task<IActionResult> Index()
        {
            var categoryTypes = await _context.FCategoryTypes.ToListAsync();
            var categories = await _context.FCategories.ToListAsync();

            var viewModel = new FCategoriesViewModel
            {
                CategoryTypes = categoryTypes,
                CategoriesByType = categories
                    .GroupBy(c => c.CaFkCategoryType)
                    .ToDictionary(g => g.Key.CtId, g => g.ToList())
            };

            if (TempData.ContainsKey("ErrorMessage"))
            {
                ModelState.AddModelError(string.Empty, TempData["ErrorMessage"].ToString());
            }

            return View(viewModel);
        }

        // GET: FCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fCategory = await _context.FCategories
                .Include(f => f.CaFkCategoryType)
                .FirstOrDefaultAsync(m => m.CaId == id);
            if (fCategory == null)
            {
                return NotFound();
            }

            return View(fCategory);
        }

        // GET: FCategories/Create
        public IActionResult Create()
        {
            ViewData["CaFkCategoryTypeId"] = new SelectList(_context.FCategoryTypes, "CtId", "CtName");
            return View();
        }

        // POST: FCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CaId,CaFkCategoryTypeId,CaName")] FCategory fCategory)
        {
            ModelState.Remove("CaFkCategoryType");
            if (ModelState.IsValid)
            {
                _context.Add(fCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CaFkCategoryTypeId"] = new SelectList(_context.FCategoryTypes, "CtId", "CtName", fCategory.CaFkCategoryTypeId);
            return View(fCategory);
        }

        // GET: FCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fCategory = await _context.FCategories.FindAsync(id);
            if (fCategory == null)
            {
                return NotFound();
            }
            ViewData["CaFkCategoryTypeId"] = new SelectList(_context.FCategoryTypes, "CtId", "CtName", fCategory.CaFkCategoryTypeId);
            return View(fCategory);
        }

        // POST: FCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CaId,CaFkCategoryTypeId,CaName")] FCategory fCategory)
        {
            if (id != fCategory.CaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FCategoryExists(fCategory.CaId))
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
            ViewData["CaFkCategoryTypeId"] = new SelectList(_context.FCategoryTypes, "CtId", "CtName", fCategory.CaFkCategoryTypeId);
            return View(fCategory);
        }

        // GET: FCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fCategory = await _context.FCategories
                .Include(f => f.CaFkCategoryType)
                .FirstOrDefaultAsync(m => m.CaId == id);
            if (fCategory == null)
            {
                return NotFound();
            }

            return View(fCategory);
        }

        // POST: FCategories/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fCategory = await _context.FCategories.FindAsync(id);
            _context.FCategories.Remove(fCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int categoryTypeId)
        {
            var category = await _context.FCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            List<FClient> clients = await _context.FClients.Where(c => c.CFkMediaId == id).ToListAsync();
            List<FIntervention> inters = await _context.FInterventions.Where(i => i.IFkCategoryId == id).ToListAsync();
            List<FInvoice> invoices = await _context.FInvoices.Where(c => c.InFkPaymentId == id).ToListAsync();
            List<FPurchase> pursSup = await _context.FPurchases.Where(c => c.PFkSupplierId == id).ToListAsync();
            List<FPurchase> pursPay = await _context.FPurchases.Where(c => c.PFkPaymentId == id).ToListAsync();
            if (clients.Any() || inters.Any() || invoices.Any() || pursSup.Any() || pursPay.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete category as there are associated products.";
                //ModelState.AddModelError(string.Empty, "Cannot delete category as there are associated products.");
                return RedirectToAction(nameof(Index));
            }

            _context.FCategories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool FCategoryExists(int id)
        {
            return _context.FCategories.Any(e => e.CaId == id);
        }
    }
}
