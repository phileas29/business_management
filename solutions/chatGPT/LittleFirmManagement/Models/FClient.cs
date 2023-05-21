using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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

public class CityUtility
{
    private static List<City> Cities = new List<City>();
    public static bool ValidateCity(string town, out City city)
    {
        LoadCitiesFromJson();
        // Find the city with the matching name
        city = Cities.FirstOrDefault(c => c.Nom.Equals(town, StringComparison.OrdinalIgnoreCase));

        if (city != null)
            return true;
        else
            return false;
    }

    private static void LoadCitiesFromJson()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Ignore case when deserializing JSON properties
        };
        // Load and deserialize the "towns.json" file into a list of City objects
        var json = File.ReadAllText("towns.json");
        Cities = JsonSerializer.Deserialize<List<City>>(json, options);
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public class City
    {
        public string Nom { get; set; }
        public string Code { get; set; }
        public string CodeDepartement { get; set; }
        public List<string> CodesPostaux { get; set; }
    }
    public static List<City> GetMatchingCities(string input)
    {
        CityUtility.LoadCitiesFromJson();
        List<City> matchingCities = CityUtility.Cities
            .Where(city => city.Nom.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            .Take(10) // Limit the number of suggestions
            .ToList();

        return matchingCities;
    }
}