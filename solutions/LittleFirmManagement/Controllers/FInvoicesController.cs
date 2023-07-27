using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;

namespace LittleFirmManagement.Controllers
{
    public class FInvoicesController : Controller
    {
        private readonly FirmContext _context;

        public FInvoicesController(FirmContext context)
        {
            _context = context;
        }

        private void PrepareViewData()
        {
            var pendingInvoices = _context.FInterventions
                .Where(i => i.IFkInvoice != null && i.IFkInvoice.InCreditDate == null)
                .OrderByDescending(i => i.IFkInvoiceId)
                .Select(i => new
                {
                    i.IFkInvoice.InInvoiceId,
                    i.IFkClient.CName,
                    IDate = i.IDate.ToShortDateString(),
                    InInvoiceDate = i.IFkInvoice.InInvoiceDate.ToShortDateString(),
                    InReceiptDate = i.IFkInvoice.InReceiptDate.Value.ToShortDateString(),
                    InCreditDate = i.IFkInvoice.InCreditDate.Value.ToShortDateString(),
                    i.IDescription,
                    i.IFkInvoice.InFkPayment.CaName,
                    i.IFkInvoice.InAmount
                })
                .ToList();

            List<SelectListItem> subjectsWithNull = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a subject",
                    Value = "",
                    Selected = true
                },
                new SelectListItem
                {
                    Text = "reception",
                    Value = "0"
                },
                new SelectListItem
                {
                    Text = "credit",
                    Value = "1"
                }
            };

            ViewData["Data"] = pendingInvoices;
            ViewData["SubjectId"] = new SelectList(subjectsWithNull, "Value", "Text", "");
        }

        public IActionResult Pending()
        {
            PrepareViewData();

            return View();
        }

        // POST: FPurchases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pending(PendingInvoicesViewModel pendingInvoicesViewModel)
        {
            if (ModelState.IsValid)
            {
                List<FInvoice> invoices = _context.FInvoices.Where(i => pendingInvoicesViewModel.InvoicesSelected.Contains(i.InInvoiceId ?? -1)).ToList();
                if (invoices != null)
                {
                    foreach (var invoice in invoices)
                    {
                        if (pendingInvoicesViewModel.SubjectId == 0)
                        {
                            if (pendingInvoicesViewModel.ActionDate < invoice.InInvoiceDate)
                                ModelState.AddModelError("ActionDate", "Please enter a receipt date greater than invoice date");
                        }
                        else if (pendingInvoicesViewModel.SubjectId == 1)
                        {
                            if (pendingInvoicesViewModel.ActionDate < invoice.InReceiptDate)
                                ModelState.AddModelError("ActionDate", "Please enter a credit date greater than receipt date");
                            if (invoice.InReceiptDate == null)
                                ModelState.AddModelError("ActionDate", "You can't enter a credit date if no receipt date is set");
                        }
                    }
                    if (ModelState.IsValid)
                    {
                        foreach (var invoice in invoices)
                        {
                            if (pendingInvoicesViewModel.SubjectId == 0)
                                invoice.InReceiptDate ??= pendingInvoicesViewModel.ActionDate;
                            else if (pendingInvoicesViewModel.SubjectId == 1)
                                invoice.InCreditDate ??= pendingInvoicesViewModel.ActionDate;
                        }
                        _context.UpdateRange(invoices);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            PrepareViewData();
            return View();
        }

        //GET: FInvoices
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

        private void PrepareViewData(ref FInvoiceCreateViewModel model, int id)
        {
            // Create a new instance of FIntervention and set default values
            model.Invoice ??= new FInvoice()
            {
                InInvoiceId = _context.FInvoices.Max(i => i.InInvoiceId).GetValueOrDefault(-1) + 1,
                InInvoiceDate = DateTime.UtcNow,
                InReceiptDate = DateTime.UtcNow
            };
            List<SelectListItem> paymentsWithNull = _context.FCategories
                .Where(c => c.CaFkCategoryType.CtName == "paiement")
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            paymentsWithNull.Insert(0,new SelectListItem { Text = "Select a payment", Value = "", Selected = true });
            model.Payments ??= new SelectList(paymentsWithNull, "Value", "Text", "");
            int clientId = id;
            model.ClientId = id;
            model.Client ??= _context.FClients
                .First(c => c.CId == clientId);

            model.Interventions ??= new MultiSelectList(
                _context.FInterventions
                .Where(i => i.IFkClientId == clientId && i.IFkInvoiceId == null)
                .OrderByDescending(i => i.IDate),
                "IId",
                "CombinedDateAndDescription",
                new[] { 
                    _context.FInterventions
                    .Where(i => i.IFkClientId == clientId && i.IFkInvoiceId == null)
                    .OrderByDescending(i => i.IDate)
                    .Select(i=>i.IId)
                    .First()
                    .ToString()
                }
            );
        }

        // GET: FInvoices/Create
        public IActionResult Create(int id, List<int> iids)
        {
            FInvoiceCreateViewModel model = new();
            PrepareViewData(ref model, id);
            return View(model);
        }

        // POST: FInvoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(FInvoiceCreateViewModel model)
        {
            ModelState.Remove("Payments");
            ModelState.Remove("Interventions");
            ModelState.Remove("Client");
            ModelState.Remove("Invoice.InFkPayment");
            if (ModelState.IsValid)
            {
                model.Invoice.InIsEligibleDeferredTaxCredit = model.InIsEligibleDeferredTaxCredit;
                model.Invoice.InInvoiceDate = DateTime.SpecifyKind(model.Invoice.InInvoiceDate, DateTimeKind.Utc);
                model.Invoice.InReceiptDate = model.Invoice.InReceiptDate.HasValue ? DateTime.SpecifyKind(model.Invoice.InReceiptDate.Value, DateTimeKind.Utc) : null;
                model.Invoice.InCreditDate = model.Invoice.InCreditDate.HasValue ? DateTime.SpecifyKind(model.Invoice.InCreditDate.Value, DateTimeKind.Utc) : null;
                _context.Add(model.Invoice);
                await _context.SaveChangesAsync();

                List<FIntervention> fInterventions = _context.FInterventions
                    .Where(i => model.SelectedInterventions
                    .Contains(i.IId))
                    .ToList();
                foreach (FIntervention inter in fInterventions)
                    inter.IFkInvoiceId = model.Invoice.InId;
                _context.UpdateRange(fInterventions);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            PrepareViewData(ref model, model.ClientId);
            return View(model);
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
