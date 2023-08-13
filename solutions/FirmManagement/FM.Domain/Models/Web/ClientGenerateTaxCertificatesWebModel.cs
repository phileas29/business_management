using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Models.Web
{
    public class ClientGenerateTaxCertificatesWebModel
    {
        public IEnumerable<SelectListItem>? CivilYear { get; set; }
        public int CivilYearId { get; set; }
    }
}
