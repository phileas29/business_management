using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LittleFirmManagement.Models;

[Table("f_client")]
public partial class FClient
{
    [Key]
    [Column("c_id")]
    public int CId { get; set; }

    [Column("c_fk_media_id")]
    public int? CFkMediaId { get; set; }

    [Column("c_fk_city_id")]
    public int CFkCityId { get; set; }

    [Column("c_fk_birth_city_id")]
    public int? CFkBirthCityId { get; set; }

    [Column("c_name", TypeName = "character varying")]
    public string CName { get; set; } = null!;

    [Column("c_firstname", TypeName = "character varying")]
    public string CFirstname { get; set; } = null!;

    [Column("c_address", TypeName = "character varying")]
    public string? CAddress { get; set; }

    [Column("c_email", TypeName = "character varying")]
    public string? CEmail { get; set; }

    [Column("c_phone_fixed", TypeName = "character varying")]
    public string? CPhoneFixed { get; set; }

    [Column("c_phone_cell", TypeName = "character varying")]
    public string? CPhoneCell { get; set; }

    [Column("c_is_pro", TypeName = "boolean")]
    public bool CIsPro { get; set; }

    [Column("c_location_long")]
    [Precision(9, 7)]
    public decimal? CLocationLong { get; set; }

    [Column("c_location_lat")]
    [Precision(9, 7)]
    public decimal? CLocationLat { get; set; }

    [Column("c_distance")]
    [Precision(3, 1)]
    public decimal? CDistance { get; set; }

    [Column("c_travel_time")]
    public int? CTravelTime { get; set; }

    [Column("c_urssaf_uuid", TypeName = "character varying")]
    public string? CUrssafUuid { get; set; }

    [Column("c_is_man", TypeName = "boolean")]
    public bool? CIsMan { get; set; }

    [NotMapped]
    public bool IsMan
    {
        get { return CIsMan ?? false; }
        set { CIsMan = value; }
    }

    [Column("c_birth_name", TypeName = "character varying")]
    public string? CBirthName { get; set; }

    [Column("c_birth_country_code")]
    public int? CBirthCountryCode { get; set; }

    [Column("c_birth_date")]
    public DateOnly? CBirthDate { get; set; }

    [Column("c_bic", TypeName = "character varying")]
    public string? CBic { get; set; }

    [Column("c_iban", TypeName = "character varying")]
    public string? CIban { get; set; }

    [Column("c_account_holder", TypeName = "character varying")]
    public string? CAccountHolder { get; set; }

    [ForeignKey("CFkBirthCityId")]
    [InverseProperty("FClientCFkBirthCities")]
    public virtual FCity? CFkBirthCity { get; set; }

    [ForeignKey("CFkCityId")]
    [InverseProperty("FClientCFkCities")]
    public virtual FCity CFkCity { get; set; } = null!;

    [ForeignKey("CFkMediaId")]
    [InverseProperty("FClients")]
    public virtual FCategory? CFkMedia { get; set; }

    [InverseProperty("IFkClient")]
    public virtual ICollection<FIntervention> FInterventions { get; set; } = new List<FIntervention>();
}
