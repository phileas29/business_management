using FM.Domain.Models.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Models.Web;

public class ClientIndexWebModel
{
    public string? NameSearch { get; set; }
    public string? FirstnameSearch { get; set; }
    public int? CitySearch { get; set; }
    public IEnumerable<SelectListItem>? Cities { get; set; }
    public List<FClient>? ClientsGPS { get; set; }
    public List<FClient>? Clients { get; set; }
    public int TotalClients { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    public int Page { get; set; } = 1;
}