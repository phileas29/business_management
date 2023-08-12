﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Models.Web;

public class ClientWebModel
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
    public DateTime? CBirthDate { get; set; }
    [StringLength(100)]
    public string? BirthCityInput { get; set; }
    [StringLength(100)]
    public string? CBic { get; set; }
    [StringLength(100)]
    public string? CIban { get; set; }
    [StringLength(100)]
    public string? CAccountHolder { get; set; }
    public IEnumerable<SelectListItem>? Medias { get; set; }
    public bool EnableUrssafPayment { get; set; }
}