using LittleFirmManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LittleFirmManagement.Controllers
{
    public class OutstandingInvoicesController : Controller
    {
        private readonly FirmContext _context;

        public OutstandingInvoicesController(FirmContext context)
        {
            _context = context;
        }

        private void PrepareViewData()
        {
            var outstandingInvoices = _context.FInterventions
                .Where(i => i.IFkInvoice != null && i.IFkInvoice.InCreditDate == null)
                .OrderByDescending(i => i.IFkInvoiceId)
                .Select(i => new
                {
                    i.IFkInvoice.InInvoiceId,
                    i.IFkClient.CName,
                    i.IDate,
                    i.IFkInvoice.InInvoiceDate,
                    i.IFkInvoice.InReceiptDate,
                    i.IFkInvoice.InCreditDate,
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

            ViewData["Data"] = outstandingInvoices;
            ViewData["SubjectId"] = new SelectList(subjectsWithNull, "Value", "Text", "");
        }

        public IActionResult Index()
        {
            PrepareViewData();

            return View();
        }

        // POST: FPurchases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OutstandingInvoicesViewModel outstandingInvoicesViewModel)
        {
            if (ModelState.IsValid)
            {
                List<FInvoice> invoices = _context.FInvoices.Where(i => outstandingInvoicesViewModel.InvoicesSelected.Contains(i.InInvoiceId??-1)).ToList();
                if (invoices != null)
                {
                    foreach (var invoice in invoices)
                    {
                        if (outstandingInvoicesViewModel.SubjectId == 0)
                        {
                            if (outstandingInvoicesViewModel.ActionDate < invoice.InInvoiceDate)
                                ModelState.AddModelError("ActionDate", "Please enter a receipt date greater than invoice date");
                        }
                        else if (outstandingInvoicesViewModel.SubjectId == 1)
                        {
                            if (outstandingInvoicesViewModel.ActionDate < invoice.InReceiptDate)
                                ModelState.AddModelError("ActionDate", "Please enter a credit date greater than receipt date");
                            if (invoice.InReceiptDate==null)
                                ModelState.AddModelError("ActionDate", "You can't enter a credit date if no receipt date is set");
                        }
                    }
                    if (ModelState.IsValid)
                    {
                        foreach (var invoice in invoices)
                        {
                            if (outstandingInvoicesViewModel.SubjectId == 0)
                                invoice.InReceiptDate ??= outstandingInvoicesViewModel.ActionDate;
                            else if (outstandingInvoicesViewModel.SubjectId == 1)
                                invoice.InCreditDate ??= outstandingInvoicesViewModel.ActionDate;
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
    }
}
