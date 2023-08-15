using FM.Domain.Models.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Models.Web
{
    public class InterventionCreateWebModel
    {
        public IEnumerable<SelectListItem>? Activities { get; set; }
        public int Choice { get; set; }
        public FClient? Client { get; set; }
        public int ClientId { get; set; }
        public int CategoryId { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public int NbRoundTrip { get; set; }
    }
}