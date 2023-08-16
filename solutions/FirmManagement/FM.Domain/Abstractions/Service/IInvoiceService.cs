using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Abstractions.Service
{
    public interface IInvoiceService
    {
        public Task<SelectList> GetYearsSelectListAsync();
        public Task<int> PutInvoiceAsync(FInvoice fInvoice);
        public FInvoice GetRepositoryInvoiceFromWebModel(InvoiceCreateWebModel wInvoice);
        public Task<InvoiceCreateWebModel> SetInvoiceWebModelAsync(InvoiceCreateWebModel? wInvoice, int id = 0, int iid = 0);
    }
}
