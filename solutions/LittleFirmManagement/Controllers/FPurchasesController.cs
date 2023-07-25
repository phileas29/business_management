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

        private void PrepareViewData()
        {
            List<SelectListItem> categoriesWithNull = _context.FCategories
                .Where(c => c.CaFkCategoryType.CtName == "achat")
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            categoriesWithNull.Add(new SelectListItem { Text = "Select a category", Value = "", Selected = true });

            List<SelectListItem> paymentsWithNull = _context.FCategories
                .Where(c => c.CaFkCategoryType.CtName == "paiement")
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            paymentsWithNull.Add(new SelectListItem { Text = "Select a payment", Value = "", Selected = true });

            List<SelectListItem> suppliersWithNull = _context.FCategories
                .Where(c => c.CaFkCategoryType.CtName == "fournisseur")
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            suppliersWithNull.Add(new SelectListItem { Text = "Select a supplier", Value = "", Selected = true });

            ViewData["PFkCategoryId"] = new SelectList(categoriesWithNull, "Value", "Text", "");
            ViewData["PFkPaymentId"] = new SelectList(paymentsWithNull, "Value", "Text", "");
            ViewData["PFkSupplierId"] = new SelectList(suppliersWithNull, "Value", "Text", "");
        }

            // GET: FPurchases/Create
        public IActionResult Create()
        {
            PrepareViewData();
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

            PrepareViewData();
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
