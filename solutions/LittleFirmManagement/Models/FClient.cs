using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
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

public class FClientUtility
{
    private static List<FClientCreateCityBusinessModel> Cities = new();
    public static bool ValidateCity(string town, out FClientCreateCityBusinessModel city)
    {
        LoadCitiesFromJson();
        // Find the city with the matching name
        city = Cities.FirstOrDefault(c => c.Nom.Equals(town, StringComparison.OrdinalIgnoreCase));

        if (city != null)
            return true;
        else
            return false;
    }
    public async static Task<FClient> ValidateClient(FClientCreateViewModel viewModel, FirmContext _context)
    {
        FClientCreateCityBusinessModel city;

        if (ValidateCity(viewModel.Town, out city))
        {
            FCity cityDb = _context.FCities.Where(c => c.CiInseeCode == Int32.Parse(city.Code)).FirstOrDefault();
            if (cityDb == null)
            {
                _context.Add(new FCity { CiPostalCode = city.CodesPostaux[0], CiName = city.Nom.ToUpper(), CiInseeCode = Int32.Parse(city.Code), CiDepartCode = Int32.Parse(city.CodeDepartement) });
                await _context.SaveChangesAsync();
                viewModel.CFkCityId = _context.FCities.Where(c => c.CiName.ToLower() == city.Nom.ToLower()).Select(c => c.CiId).FirstOrDefault();
            }
            else
                viewModel.CFkCityId = cityDb.CiId;
            _context.SaveChanges();
        }

        if (ValidateCity(viewModel.BirthCityInput, out city))
        {
            FCity cityDb = _context.FCities.Where(c => c.CiInseeCode == Int32.Parse(city.Code)).FirstOrDefault();
            if (cityDb == null)
            {
                _context.Add(new FCity { CiPostalCode = city.CodesPostaux[0], CiName = city.Nom.ToUpper(), CiInseeCode = Int32.Parse(city.Code), CiDepartCode = Int32.Parse(city.CodeDepartement) });
                viewModel.CFkBirthCityId = _context.FCities.Where(c => c.CiName.ToLower() == city.Nom.ToLower()).Select(c => c.CiId).FirstOrDefault();
            }
            else
                viewModel.CFkBirthCityId = cityDb.CiId;
            _context.SaveChanges();
        }


        var client = new FClient
        {
            CFkMediaId = viewModel.CFkMediaId,
            CFkCityId = viewModel.CFkCityId,
            CFkBirthCityId = viewModel.CFkBirthCityId,
            CName = viewModel.CName,
            CFirstname = viewModel.CFirstname,
            CAddress = viewModel.CAddress,
            CEmail = viewModel.CEmail,
            CPhoneFixed = viewModel.CPhoneFixed,
            CPhoneCell = viewModel.CPhoneCell,
            CIsPro = viewModel.CIsPro,
            CLocationLong = viewModel.CLocationLong,
            CLocationLat = viewModel.CLocationLat,
            CDistance = viewModel.CDistance,
            CTravelTime = viewModel.CTravelTime,
            CUrssafUuid = viewModel.CUrssafUuid,
            CIsMan = viewModel.CIsMan,
            CBirthName = viewModel.CBirthName,
            CBirthDate = viewModel.CBirthDate,
            CBic = viewModel.CBic,
            CIban = viewModel.CIban,
            CAccountHolder = viewModel.CAccountHolder
        };


        return client;
    }

    private static void LoadCitiesFromJson()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Ignore case when deserializing JSON properties
        };
        // Load and deserialize the "towns.json" file into a list of City objects
        var json = File.ReadAllText("towns.json");
        Cities = JsonSerializer.Deserialize<List<FClientCreateCityBusinessModel>>(json, options);
    }
    public static List<FClientCreateCityBusinessModel> GetMatchingCities(string input)
    {
        LoadCitiesFromJson();
        List<FClientCreateCityBusinessModel> matchingCities = Cities
            .Where(city => city.Nom.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            .Take(10) // Limit the number of suggestions
            .ToList();

        return matchingCities;
    }


}

public class FClientCreateCityBusinessModel
{
    public string Nom { get; set; }
    public string Code { get; set; }
    public string CodeDepartement { get; set; }
    public List<string> CodesPostaux { get; set; }
}
public class FClientIndexViewModel
{
    public string? NameSearch { get; set; }
    public string? FirstnameSearch { get; set; }
    public int? CitySearch { get; set; }
    public IEnumerable<SelectListItem> Cities { get; set; }
    public IQueryable<FClient> ClientsGPS { get; set; }
    public List<FClient> Clients { get; set; }
    public int TotalClients { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    public int Page { get; set; } = 1;
}


public class FClientCreateViewModel
{
    public int? CFkMediaId { get; set; }
    public int CFkCityId { get; set; }
    public int? CFkBirthCityId { get; set; }
    [StringLength(100)]
    public string CName { get; set; } = "";
    [StringLength(100)]
    public string CFirstname { get; set; } = "";
    [StringLength(100)]
    public string? CAddress { get; set; }
    [EmailAddress]
    public string? CEmail { get; set; }
    [Phone]
    public string? CPhoneFixed { get; set; }
    [Phone]
    public string? CPhoneCell { get; set; }
    public bool CIsPro { get; set; }
    public decimal? CLocationLong { get; set; }
    public decimal? CLocationLat { get; set; }
    [StringLength(100)]
    public string Town { get; set; } = "";
    public decimal? CDistance { get; set; }
    public int? CTravelTime { get; set; }
    [StringLength(100)]
    public string? CUrssafUuid { get; set; }
    public bool? CIsMan { get; set; }
    public bool IsMan
    {
        get { return CIsMan ?? false; }
        set { CIsMan = value; }
    }
    [StringLength(100)]
    public string? CBirthName { get; set; }
    public DateOnly? CBirthDate { get; set; }
    [StringLength(100)]
    public string? BirthCityInput { get; set; }
    [StringLength(100)]
    public string? CBic { get; set; }
    [StringLength(100)]
    public string? CIban { get; set; }
    [StringLength(100)]
    public string? CAccountHolder { get; set; }
    public IEnumerable<SelectListItem>? Medias { get; set; }
}
public class FClientGenerateTaxCertificatesViewModel
{
    public IEnumerable<SelectListItem>? CivilYear { get; set; }
    public int CivilYearId { get; set; }
}
public class FClientGenerateTaxCertificatesBusinessModel
{
    public DateTime Date { get; set; }
    public decimal Duration { get; set; }
    public int HourlyRate { get; set; }
    public int Amount { get; set; }
}