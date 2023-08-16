using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc;

namespace FM.Domain.Abstractions.Web
{
    public interface IInvoiceWeb
    {
        public Task<IActionResult> CreateAsync(int id, int iid);
        public Task<IActionResult> CreateAsync(InvoiceCreateWebModel wInvoice);
    }
}
