using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FM.Domain.Models.Repository;

[Table("f_city")]
public partial class FCity
{
    [Key]
    [Column("ci_id")]
    public int CiId { get; set; }

    [Column("ci_postal_code", TypeName = "character varying")]
    public string CiPostalCode { get; set; } = null!;

    [Column("ci_name", TypeName = "character varying")]
    public string CiName { get; set; } = null!;

    [Column("ci_insee_code")]
    public int? CiInseeCode { get; set; }

    [Column("ci_depart_code")]
    public int? CiDepartCode { get; set; }

    [InverseProperty("CFkBirthCity")]
    public virtual ICollection<FClient> FClientCFkBirthCities { get; set; } = new List<FClient>();

    [InverseProperty("CFkCity")]
    public virtual ICollection<FClient> FClientCFkCities { get; set; } = new List<FClient>();
}
