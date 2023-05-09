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
    public class FInterventionsController : Controller
    {
        private readonly FirmContext _context;

        public FInterventionsController(FirmContext context)
        {
            _context = context;
        }

        // GET: FInterventions
        public async Task<IActionResult> Index()
        {
            var firmContext = _context.FInterventions.Include(f => f.IFkCategory).Include(f => f.IFkClient).Include(f => f.IFkInvoice);
            return View(await firmContext.ToListAsync());
        }

        // GET: FInterventions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fIntervention = await _context.FInterventions
                .Include(f => f.IFkCategory)
                .Include(f => f.IFkClient)
                .Include(f => f.IFkInvoice)
                .FirstOrDefaultAsync(m => m.IId == id);
            if (fIntervention == null)
            {
                return NotFound();
            }

            return View(fIntervention);
        }

        // GET: FInterventions/Create
        public IActionResult Create()
        {
            ViewData["IFkCategoryId"] = new SelectList(_context.FCategories, "CaId", "CaId");
            ViewData["IFkClientId"] = new SelectList(_context.FClients, "CId", "CId");
            ViewData["IFkInvoiceId"] = new SelectList(_context.FInvoices, "InId", "InId");
            return View();
        }

        // POST: FInterventions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IId,IFkClientId,IFkInvoiceId,IFkCategoryId,IDate,IDescription,INbRoundTrip")] FIntervention fIntervention)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fIntervention);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IFkCategoryId"] = new SelectList(_context.FCategories, "CaId", "CaId", fIntervention.IFkCategoryId);
            ViewData["IFkClientId"] = new SelectList(_context.FClients, "CId", "CId", fIntervention.IFkClientId);
            ViewData["IFkInvoiceId"] = new SelectList(_context.FInvoices, "InId", "InId", fIntervention.IFkInvoiceId);
            return View(fIntervention);
        }

        // GET: FInterventions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fIntervention = await _context.FInterventions.FindAsync(id);
            if (fIntervention == null)
            {
                return NotFound();
            }
            ViewData["IFkCategoryId"] = new SelectList(_context.FCategories, "CaId", "CaId", fIntervention.IFkCategoryId);
            ViewData["IFkClientId"] = new SelectList(_context.FClients, "CId", "CId", fIntervention.IFkClientId);
            ViewData["IFkInvoiceId"] = new SelectList(_context.FInvoices, "InId", "InId", fIntervention.IFkInvoiceId);
            return View(fIntervention);
        }

        // POST: FInterventions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IId,IFkClientId,IFkInvoiceId,IFkCategoryId,IDate,IDescription,INbRoundTrip")] FIntervention fIntervention)
        {
            if (id != fIntervention.IId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fIntervention);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FInterventionExists(fIntervention.IId))
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
            ViewData["IFkCategoryId"] = new SelectList(_context.FCategories, "CaId", "CaId", fIntervention.IFkCategoryId);
            ViewData["IFkClientId"] = new SelectList(_context.FClients, "CId", "CId", fIntervention.IFkClientId);
            ViewData["IFkInvoiceId"] = new SelectList(_context.FInvoices, "InId", "InId", fIntervention.IFkInvoiceId);
            return View(fIntervention);
        }

        // GET: FInterventions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fIntervention = await _context.FInterventions
                .Include(f => f.IFkCategory)
                .Include(f => f.IFkClient)
                .Include(f => f.IFkInvoice)
                .FirstOrDefaultAsync(m => m.IId == id);
            if (fIntervention == null)
            {
                return NotFound();
            }

            return View(fIntervention);
        }

        // POST: FInterventions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fIntervention = await _context.FInterventions.FindAsync(id);
            _context.FInterventions.Remove(fIntervention);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FInterventionExists(int id)
        {
            return _context.FInterventions.Any(e => e.IId == id);
        }
    }
}