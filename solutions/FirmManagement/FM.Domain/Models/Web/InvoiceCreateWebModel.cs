using FM.Domain.Models.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Models.Web
{
    public class InvoiceCreateWebModel
    {
        public int Choice { get; set; }
        public IEnumerable<SelectListItem>? Payments { get; set; }
        public List<int>? SelectedInterventions { get; set; }
        public int ClientId { get; set; }
        public int InterventionId { get; set; }
        public FClient? Client { get; set; }
        public MultiSelectList? Interventions { get; set; }
        public bool IsEligibleDeferredTaxCredit { get; set; }
        public int PaymentId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? CreditDate { get; set; }
        public int Amount { get; set; }
        public int? InvoiceId { get; set; }
        public string? UrssafPaymentRequestUuid { get; set; }
    }
}
