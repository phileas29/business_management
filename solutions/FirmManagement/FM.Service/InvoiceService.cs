using FM.Domain.Abstractions.Repository;
using FM.Domain.Abstractions.Service;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Service;
using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IInterventionRepository _interventionRepository;
        private readonly ICategoryService _categoryService;
        public InvoiceService(IInvoiceRepository invoiceRepository, IClientRepository clientRepository, ICategoryService categoryService, IInterventionRepository interventionRepository)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _interventionRepository = interventionRepository;
            _categoryService = categoryService;
        }

        public FInvoice GetRepositoryInvoiceFromWebModel(InvoiceCreateWebModel wInvoice)
        {
            FInvoice fInvoice = new()
            {
                InInvoiceId = wInvoice.InvoiceId,
                InInvoiceDate = wInvoice.InvoiceDate,
                InReceiptDate = wInvoice.ReceiptDate,
                InIsEligibleDeferredTaxCredit = wInvoice.IsEligibleDeferredTaxCredit,
                InFkPaymentId = wInvoice.PaymentId,
                InCreditDate = wInvoice.CreditDate,
                InAmount = wInvoice.Amount,
                InUrssafPaymentRequestUuid = wInvoice.UrssafPaymentRequestUuid,
            };
            return fInvoice;
        }

        public async Task<SelectList> GetYearsSelectListAsync()
        {
            List<int> years = await _invoiceRepository.SelectYearsFromInvoicesAsync();
            List<SelectListItem> yearsWithNull = years
                .Select(y => new SelectListItem
                {
                    Text = y.ToString(),
                    Value = y.ToString()
                })
                .ToList();
            yearsWithNull.Insert(0, new SelectListItem { Text = "Select a year", Value = "", Selected = true });
            return new SelectList(yearsWithNull, "Value", "Text", "");
        }

        public async Task<int> PutInvoiceAsync(FInvoice fInvoice)
        {
            return await _invoiceRepository.InsertInvoiceAsync(fInvoice);
        }

        public async Task<InvoiceCreateWebModel> SetInvoiceWebModelAsync(InvoiceCreateWebModel? wInvoice, int id = 0, int iid = 0)
        {
            InvoiceCreateWebModel wInvoiceResult;
            if(wInvoice == null)
            {
                wInvoiceResult = new InvoiceCreateWebModel()
                {
                    InvoiceId = _invoiceRepository.MaxInvoiceId() + 1,
                    InvoiceDate = DateTime.UtcNow,
                    ReceiptDate = DateTime.UtcNow,
                    IsEligibleDeferredTaxCredit = true,
                    InterventionId = iid,
                    ClientId = id,
                    Amount = 60
                };
            }
            else
            {
                wInvoiceResult = new InvoiceCreateWebModel()
                {
                    InvoiceId = wInvoice.InvoiceId,
                    InvoiceDate = wInvoice.InvoiceDate,
                    ReceiptDate = wInvoice.ReceiptDate,
                    IsEligibleDeferredTaxCredit = wInvoice.IsEligibleDeferredTaxCredit,
                    InterventionId = wInvoice.InterventionId,
                    PaymentId = wInvoice.PaymentId,
                    CreditDate = wInvoice.CreditDate,
                    Amount = wInvoice.Amount,
                    UrssafPaymentRequestUuid = wInvoice.UrssafPaymentRequestUuid,
                    SelectedInterventions = wInvoice.SelectedInterventions,
                    ClientId = wInvoice.ClientId,
                };
            }
            List<InvoiceCreateServiceModel> sInterventions = await _interventionRepository.SelectUninvoicedInterventionsByClientAsync(wInvoiceResult.ClientId);
            wInvoiceResult.Interventions = new MultiSelectList(
                sInterventions,
                "Id",
                "CombinedDateAndDescription",
                wInvoiceResult.InterventionId == 0 ? Array.Empty<string>() : new[] { wInvoiceResult.InterventionId }
            );
            wInvoiceResult.Client = await _clientRepository.SelectClientByIdAndIncludeCityAsync(wInvoiceResult.ClientId);
            wInvoiceResult.Payments = await _categoryService.GetSelectListAsync("paiement");
            return wInvoiceResult;
        }
    }
}
