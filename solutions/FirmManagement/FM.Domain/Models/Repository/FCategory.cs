using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FM.Domain.Models.Repository;

[Table("f_category")]
public partial class FCategory
{
    [Key]
    [Column("ca_id")]
    public int CaId { get; set; }

    [Column("ca_fk_category_type_id")]
    public int CaFkCategoryTypeId { get; set; }

    [Column("ca_name", TypeName = "character varying")]
    public string CaName { get; set; } = null!;

    [ForeignKey("CaFkCategoryTypeId")]
    [InverseProperty("FCategories")]
    public virtual FCategoryType CaFkCategoryType { get; set; } = null!;

    [InverseProperty("CFkMedia")]
    public virtual ICollection<FClient> FClients { get; set; } = new List<FClient>();

    [InverseProperty("IFkCategory")]
    public virtual ICollection<FIntervention> FInterventions { get; set; } = new List<FIntervention>();

    [InverseProperty("InFkPayment")]
    public virtual ICollection<FInvoice> FInvoices { get; set; } = new List<FInvoice>();

    [InverseProperty("PFkCategory")]
    public virtual ICollection<FPurchase> FPurchasePFkCategories { get; set; } = new List<FPurchase>();

    [InverseProperty("PFkPayment")]
    public virtual ICollection<FPurchase> FPurchasePFkPayments { get; set; } = new List<FPurchase>();

    [InverseProperty("PFkSupplier")]
    public virtual ICollection<FPurchase> FPurchasePFkSuppliers { get; set; } = new List<FPurchase>();
}