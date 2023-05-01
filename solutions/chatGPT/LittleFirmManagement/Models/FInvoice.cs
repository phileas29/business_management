using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LittleFirmManagement.Models;

[Table("f_invoice")]
public partial class FInvoice
{
    [Key]
    [Column("in_id")]
    public int InId { get; set; }

    [Column("in_fk_payment_id")]
    public int InFkPaymentId { get; set; }

    [Column("in_invoice_id")]
    public int? InInvoiceId { get; set; }

    [Column("in_invoice_id_corrected")]
    public int? InInvoiceIdCorrected { get; set; }

    [Column("in_invoice_date")]
    public DateOnly InInvoiceDate { get; set; }

    [Column("in_receipt_date")]
    public DateOnly? InReceiptDate { get; set; }

    [Column("in_credit_date")]
    public DateOnly? InCreditDate { get; set; }

    [Column("in_amount")]
    public int InAmount { get; set; }

    [Column("in_is_eligible_deferred_tax_credit", TypeName = "bit(1)")]
    public BitArray? InIsEligibleDeferredTaxCredit { get; set; }

    [Column("in_urssaf_payment_request_uuid", TypeName = "character varying")]
    public string? InUrssafPaymentRequestUuid { get; set; }

    [InverseProperty("IFkInvoice")]
    public virtual ICollection<FIntervention> FInterventions { get; set; } = new List<FIntervention>();

    [ForeignKey("InFkPaymentId")]
    [InverseProperty("FInvoices")]
    public virtual FCategory InFkPayment { get; set; } = null!;
}
