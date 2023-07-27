﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LittleFirmManagement.Models;

[Table("f_purchase")]
public partial class FPurchase
{
    [Key]
    [Column("p_id")]
    public int PId { get; set; }

    [Column("p_fk_payment_id")]
    public int PFkPaymentId { get; set; }

    [Column("p_fk_category_id")]
    public int PFkCategoryId { get; set; }

    [Column("p_fk_supplier_id")]
    public int PFkSupplierId { get; set; }

    [Column("p_invoice_date")]
    public DateTime? PInvoiceDate { get; set; }

    [Column("p_disbursement_date")]
    public DateTime PDisbursementDate { get; set; }

    [Column("p_debit_date")]
    public DateTime? PDebitDate { get; set; }

    [Column("p_description", TypeName = "character varying")]
    public string? PDescription { get; set; }

    [Column("p_amount")]
    [Precision(6, 2)]
    public decimal PAmount { get; set; }

    [ForeignKey("PFkCategoryId")]
    [InverseProperty("FPurchasePFkCategories")]
    public virtual FCategory PFkCategory { get; set; } = null!;

    [ForeignKey("PFkPaymentId")]
    [InverseProperty("FPurchasePFkPayments")]
    public virtual FCategory PFkPayment { get; set; } = null!;

    [ForeignKey("PFkSupplierId")]
    [InverseProperty("FPurchasePFkSuppliers")]
    public virtual FCategory PFkSupplier { get; set; } = null!;
}

public class FPurchaseCreateViewModel
{
    public FPurchase Purchase { get; set; }
    public IEnumerable<SelectListItem> Categories { get; set; }
    public IEnumerable<SelectListItem> Payments { get; set; }
    public IEnumerable<SelectListItem> Suppliers { get; set; }
}
