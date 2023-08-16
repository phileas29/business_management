using FM.Domain.Abstractions.Service;
using FM.Domain.Abstractions.Web;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc;

namespace FM.Web.Controllers
{
    public class FInvoicesController : Controller, IInvoiceWeb
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IInterventionService _interventionService;
        public FInvoicesController(IInvoiceService invoiceService, IInterventionService interventionService)
        {
            _invoiceService = invoiceService;
            _interventionService = interventionService;
        }

        // GET: FInvoices/Create/id
        public async Task<IActionResult> CreateAsync(int id, int iid)
        {
            InvoiceCreateWebModel wInvoice = await _invoiceService.SetInvoiceWebModelAsync(null, id, iid);
            return View(wInvoice);
        }

        // POST: FInvoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("Payments, SelectedInterventions, ClientId, InterventionId, Client, Interventions, IsEligibleDeferredTaxCredit, PaymentId, InvoiceDate, ReceiptDate, CreditDate, Amount, InvoiceId, UrssafPaymentRequestUuid")] InvoiceCreateWebModel wInvoice)
        {
            if (ModelState.IsValid)//TODO: choice 1 to impl
            {
                FInvoice fInvoice = _invoiceService.GetRepositoryInvoiceFromWebModel(wInvoice);
                int id = await _invoiceService.PutInvoiceAsync(fInvoice);
                await _interventionService.UpdateInvoicesIdsAsync(wInvoice.SelectedInterventions, id);
                if (wInvoice.Choice == 1)
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Index", "Home");
            }
            wInvoice = await _invoiceService.SetInvoiceWebModelAsync(wInvoice);
            return View(wInvoice);
        }
    }
}
