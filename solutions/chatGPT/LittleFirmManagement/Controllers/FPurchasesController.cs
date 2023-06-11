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
    public class FPurchasesController : Controller
    {
        private readonly FirmContext _context;

        public FPurchasesController(FirmContext context)
        {
            _context = context;
        }

        // GET: FPurchases
        public async Task<IActionResult> Index()
        {
            var firmContext = _context.FPurchases.Include(f => f.PFkCategory).Include(f => f.PFkPayment).Include(f => f.PFkSupplier);
            return View(await firmContext.ToListAsync());
        }

        // GET: FPurchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fPurchase = await _context.FPurchases
                .Include(f => f.PFkCategory)
                .Include(f => f.PFkPayment)
                .Include(f => f.PFkSupplier)
                .FirstOrDefaultAsync(m => m.PId == id);
            if (fPurchase == null)
            {
                return NotFound();
            }

            return View(fPurchase);
        }

        // GET: FPurchases/Create
        public IActionResult Create()
        {
            var categoriesWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "achat").ToList();
            categoriesWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a category" });
            var paymentsWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "paiement").ToList();
            paymentsWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a payment" });
            var suppliersWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "fournisseur").OrderBy(p=>p.CaName).ToList();
            suppliersWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a supplier" });

            ViewData["PFkCategoryId"] = new SelectList(categoriesWithNull, "CaId", "CaName");
            ViewData["PFkPaymentId"] = new SelectList(paymentsWithNull, "CaId", "CaName");
            ViewData["PFkSupplierId"] = new SelectList(suppliersWithNull, "CaId", "CaName");
            return View();
        }

        // POST: FPurchases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PId,PFkPaymentId,PFkCategoryId,PFkSupplierId,PInvoiceDate,PDisbursementDate,PDebitDate,PDescription,PAmount")] FPurchase fPurchase)
        {
            ModelState.Remove("PFkCategory");
            ModelState.Remove("PFkPayment");
            ModelState.Remove("PFkSupplier");
            if (fPurchase.PFkCategoryId == -1)
                ModelState.AddModelError("PFkCategoryId", "Please select a category.");
            if (fPurchase.PFkPaymentId == -1)
                ModelState.AddModelError("PFkPaymentId", "Please select a payment.");
            if (fPurchase.PFkSupplierId == -1)
                ModelState.AddModelError("PFkSupplierId", "Please select a supplier.");
            if (ModelState.IsValid)
            {
                fPurchase.PDisbursementDate = DateTime.SpecifyKind(fPurchase.PDisbursementDate, DateTimeKind.Utc);
                if (fPurchase.PDebitDate.HasValue)
                    fPurchase.PDebitDate = DateTime.SpecifyKind(fPurchase.PDebitDate.Value, DateTimeKind.Utc);
                if (fPurchase.PInvoiceDate.HasValue)
                    fPurchase.PInvoiceDate = DateTime.SpecifyKind(fPurchase.PInvoiceDate.Value, DateTimeKind.Utc);
                _context.Add(fPurchase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var categoriesWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "achat").ToList();
            categoriesWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a category" });
            var paymentsWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "paiement").ToList();
            paymentsWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a payment" });
            var suppliersWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "fournisseur").OrderBy(p => p.CaName).ToList();
            suppliersWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a supplier" });

            ViewData["PFkCategoryId"] = new SelectList(categoriesWithNull, "CaId", "CaName");
            ViewData["PFkPaymentId"] = new SelectList(paymentsWithNull, "CaId", "CaName");
            ViewData["PFkSupplierId"] = new SelectList(suppliersWithNull, "CaId", "CaName");
            return View(fPurchase);
        }

        // GET: FPurchases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fPurchase = await _context.FPurchases.FindAsync(id);
            if (fPurchase == null)
            {
                return NotFound();
            }
            ViewData["PFkCategoryId"] = new SelectList(_context.FCategories, "CaId", "CaId", fPurchase.PFkCategoryId);
            ViewData["PFkPaymentId"] = new SelectList(_context.FCategories, "CaId", "CaId", fPurchase.PFkPaymentId);
            ViewData["PFkSupplierId"] = new SelectList(_context.FCategories, "CaId", "CaId", fPurchase.PFkSupplierId);
            return View(fPurchase);
        }

        // POST: FPurchases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PId,PFkPaymentId,PFkCategoryId,PFkSupplierId,PInvoiceDate,PDisbursementDate,PDebitDate,PDescription,PAmount")] FPurchase fPurchase)
        {
            if (id != fPurchase.PId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fPurchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FPurchaseExists(fPurchase.PId))
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
            ViewData["PFkCategoryId"] = new SelectList(_context.FCategories, "CaId", "CaId", fPurchase.PFkCategoryId);
            ViewData["PFkPaymentId"] = new SelectList(_context.FCategories, "CaId", "CaId", fPurchase.PFkPaymentId);
            ViewData["PFkSupplierId"] = new SelectList(_context.FCategories, "CaId", "CaId", fPurchase.PFkSupplierId);
            return View(fPurchase);
        }

        // GET: FPurchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fPurchase = await _context.FPurchases
                .Include(f => f.PFkCategory)
                .Include(f => f.PFkPayment)
                .Include(f => f.PFkSupplier)
                .FirstOrDefaultAsync(m => m.PId == id);
            if (fPurchase == null)
            {
                return NotFound();
            }

            return View(fPurchase);
        }

        // POST: FPurchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fPurchase = await _context.FPurchases.FindAsync(id);
            _context.FPurchases.Remove(fPurchase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FPurchaseExists(int id)
        {
            return _context.FPurchases.Any(e => e.PId == id);
        }
    }
}
