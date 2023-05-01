using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LittleFirmManagement.Models;

[Table("f_intervention")]
public partial class FIntervention
{
    [Key]
    [Column("i_id")]
    public int IId { get; set; }

    [Column("i_fk_client_id")]
    public int IFkClientId { get; set; }

    [Column("i_fk_invoice_id")]
    public int? IFkInvoiceId { get; set; }

    [Column("i_fk_category_id")]
    public int IFkCategoryId { get; set; }

    [Column("i_date")]
    public DateOnly IDate { get; set; }

    [Column("i_description", TypeName = "character varying")]
    public string? IDescription { get; set; }

    [Column("i_nb_round_trip")]
    public int INbRoundTrip { get; set; }

    [ForeignKey("IFkCategoryId")]
    [InverseProperty("FInterventions")]
    public virtual FCategory IFkCategory { get; set; } = null!;

    [ForeignKey("IFkClientId")]
    [InverseProperty("FInterventions")]
    public virtual FClient IFkClient { get; set; } = null!;

    [ForeignKey("IFkInvoiceId")]
    [InverseProperty("FInterventions")]
    public virtual FInvoice? IFkInvoice { get; set; }
}
