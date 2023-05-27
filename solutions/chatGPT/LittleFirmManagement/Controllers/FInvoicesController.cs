using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace LittleFirmManagement.Controllers
{
    public class FInvoicesController : Controller
    {
        private readonly FirmContext _context;

        public FInvoicesController(FirmContext context)
        {
            _context = context;
        }

        // GET: FInvoices
        public async Task<IActionResult> Index()
        {
            var firmContext = _context.FInvoices.Include(f => f.InFkPayment);
            return View(await firmContext.ToListAsync());
        }

        // GET: FInvoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fInvoice = await _context.FInvoices
                .Include(f => f.InFkPayment)
                .FirstOrDefaultAsync(m => m.InId == id);
            if (fInvoice == null)
            {
                return NotFound();
            }

            return View(fInvoice);
        }

        // GET: FInvoices/Create
        public IActionResult Create(int CId, int IId)
        {
            var paymentsWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "paiement").ToList();
            paymentsWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a payment mode" });
            ViewData["InFkPaymentId"] = new SelectList(paymentsWithNull, "CaId", "CaName");
            ViewData["FClient"] = _context.FClients.FirstOrDefault(c => c.CId == CId);

            ViewBag.FIntervention = new MultiSelectList(_context.FInterventions.Where(i => i.IFkClientId == CId), "IId", "CombinedDateAndDescription");

            //var inters = _context.FInterventions.Where(i => i.IFkClientId == CId);
            //ViewData["FIntervention"] = new SelectList(_context.FInterventions.Where(i => i.IFkClientId == CId), "IId", "IDescription");
            return View();
        }

        // POST: FInvoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InId,InFkPaymentId,InInvoiceId,InInvoiceIdCorrected,InInvoiceDate,InReceiptDate,InCreditDate,InAmount,InIsEligibleDeferredTaxCredit,InUrssafPaymentRequestUuid")] FInvoice fInvoice, List<int> selectedInterventions, int CId)
        {
            ModelState.Remove("CId");
            ModelState.Remove("fInvoice.InFkPayment");
            if (ModelState.IsValid)
            {
                _context.Add(fInvoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var paymentsWithNull = _context.FCategories.Where(c => c.CaFkCategoryType.CtName == "paiement").ToList();
            paymentsWithNull.Insert(0, new FCategory { CaId = -1, CaName = "Select a payment mode" });
            ViewData["InFkPaymentId"] = new SelectList(paymentsWithNull, "CaId", "CaName");
            ViewData["FClient"] = _context.FClients.FirstOrDefault(c => c.CId == CId);

            ViewBag.FIntervention = new MultiSelectList(_context.FInterventions.Where(i => i.IFkClientId == CId), "IId", "CombinedDateAndDescription");
            FInvoicesViewModel fInvoiceViewModel = new FInvoicesViewModel
            {
                selectedInterventions = selectedInterventions,
                fInvoice = fInvoice
            };
            return View(fInvoiceViewModel);
        }

        // GET: FInvoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fInvoice = await _context.FInvoices.FindAsync(id);
            if (fInvoice == null)
            {
                return NotFound();
            }
            ViewData["InFkPaymentId"] = new SelectList(_context.FCategories, "CaId", "CaId", fInvoice.InFkPaymentId);
            return View(fInvoice);
        }

        // POST: FInvoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InId,InFkPaymentId,InInvoiceId,InInvoiceIdCorrected,InInvoiceDate,InReceiptDate,InCreditDate,InAmount,InIsEligibleDeferredTaxCredit,InUrssafPaymentRequestUuid")] FInvoice fInvoice)
        {
            if (id != fInvoice.InId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fInvoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FInvoiceExists(fInvoice.InId))
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
            ViewData["InFkPaymentId"] = new SelectList(_context.FCategories, "CaId", "CaId", fInvoice.InFkPaymentId);
            return View(fInvoice);
        }

        // GET: FInvoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fInvoice = await _context.FInvoices
                .Include(f => f.InFkPayment)
                .FirstOrDefaultAsync(m => m.InId == id);
            if (fInvoice == null)
            {
                return NotFound();
            }

            return View(fInvoice);
        }

        // POST: FInvoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fInvoice = await _context.FInvoices.FindAsync(id);
            _context.FInvoices.Remove(fInvoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FInvoiceExists(int id)
        {
            return _context.FInvoices.Any(e => e.InId == id);
        }
    }
}
