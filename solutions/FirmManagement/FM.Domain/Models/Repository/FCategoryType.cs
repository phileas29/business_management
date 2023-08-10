using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FM.Domain.Models.Repository;

[Table("f_category_type")]
public partial class FCategoryType
{
    [Key]
    [Column("ct_id")]
    public int CtId { get; set; }

    [Column("ct_name", TypeName = "character varying")]
    public string CtName { get; set; } = null!;

    [InverseProperty("CaFkCategoryType")]
    public virtual ICollection<FCategory> FCategories { get; set; } = new List<FCategory>();
}
