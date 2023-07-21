using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace LittleFirmManagement.Controllers
{
    public class FClientsController : Controller
    {
        private readonly FirmContext _context;
        private readonly IConverter _converter;

        public FClientsController(FirmContext context, IConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        // GET: FClients
        public async Task<IActionResult> Index(string nameSearch="", string firstnameSearch = "", int citySearch = -1, int page = 1, int pageSize = 10)
        {
            var clients = _context.FClients.OrderBy(c=>c.CName).Include(f => f.CFkBirthCity).Include(f => f.CFkCity).Include(f => f.CFkMedia).AsQueryable();

            if (!string.IsNullOrEmpty(nameSearch))
            {
                clients = clients.Where(c => c.CName.ToLower().Contains(nameSearch.ToLower()));
            }

            if (!string.IsNullOrEmpty(firstnameSearch))
            {
                clients = clients.Where(c => c.CFirstname.ToLower().Contains(firstnameSearch.ToLower()));
            }

            if (0 < citySearch)
            {
                clients = clients.Where(c => c.CFkCity.CiId == citySearch);
            }


            // Calculate pagination values
            int totalClients = clients.Count();
            int totalPages = (int)Math.Ceiling((double)totalClients / pageSize);

            // Apply pagination
            var clientsSelectedPage = clients.Skip((page - 1) * pageSize).Take(pageSize);

            // Pass the paginated clients and pagination data to the view
            if ((nameSearch == null || nameSearch == "") && (firstnameSearch == null || firstnameSearch == "") && citySearch == -1)
                ViewBag.ClientsGPS = null;
            else
                ViewBag.ClientsGPS = clients.Except(clientsSelectedPage);

            //ViewBag.SearchEnabled = searchEnabled;
            ViewBag.Clients = clientsSelectedPage.ToList();
            ViewBag.TotalClients = totalClients;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            ViewData["NameSearch"] = nameSearch;
            ViewData["FirstnameSearch"] = firstnameSearch;
            ViewData["CitySearch"] = citySearch;
            ViewData["Cities"] = _context.FClients.Select(c => c.CFkCity).Distinct().OrderBy(c=>c.CiName).ToList();

            return View();
        }

        // GET: FClients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fClient = await _context.FClients
                .Include(f => f.CFkBirthCity)
                .Include(f => f.CFkCity)
                .Include(f => f.CFkMedia)
                .FirstOrDefaultAsync(m => m.CId == id);
            if (fClient == null)
            {
                return NotFound();
            }

            return View(fClient);
        }

        private void PrepareViewData()
        {
            List<SelectListItem> mediasWithNull = _context.FCategories
                .Where(c => c.CaFkCategoryType.CtName == "média")
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            mediasWithNull.Add(new SelectListItem { Text = "Select a media", Value = "", Selected = true });
            ViewData["CFkMediaId"] = new SelectList(mediasWithNull, "Value", "Text", "");
        }

        // GET: FClients/Create
        public IActionResult Create()
        {
            PrepareViewData();
            //var viewModel = new FClientsViewModel
            //{
            //    CName = "Default Name",
            //    CFirstname = "Default Firstname",
            //    CEmail = "default@example.com",
            //    CPhoneFixed = "1234567890",
            //    CPhoneCell = "9876543210",
            //    CFkMediaId = _context.FCategories
            //    .Where(c => c.CaFkCategoryType.CtName == "média" && c.CaName == "journal").Select(m => m.CaId).First()
            //};
            return View();
        }

        // POST: FClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CFkMediaId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder,Town,BirthCityInput")] FClientsViewModel vClient, bool saveAndExit)
        {
            ModelState.Remove("CFkCity");
            ModelState.Remove("CFkBirthCityId");
            if (ModelState.IsValid)
            {
                FClient fClient = FClientUtility.ValidateClient(vClient, _context).Result;
                _context.Add(fClient);
                await _context.SaveChangesAsync();
                if (saveAndExit)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Create", "FInterventions", new { id = fClient.CId });
            }
            PrepareViewData();
            return View(vClient);
        }

        // GET: FClients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fClient = await _context.FClients.FindAsync(id);
            if (fClient == null)
            {
                return NotFound();
            }
            ViewData["CFkBirthCityId"] = new SelectList(_context.FCities, "CiId", "CiId", fClient.CFkBirthCityId);
            ViewData["CFkCityId"] = new SelectList(_context.FCities, "CiId", "CiId", fClient.CFkCityId);
            ViewData["CFkMediaId"] = new SelectList(_context.FCategories, "CaId", "CaId", fClient.CFkMediaId);
            return View(fClient);
        }

        // POST: FClients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CId,CFkMediaId,CFkCityId,CFkBirthCityId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder")] FClient fClient)
        {
            if (id != fClient.CId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fClient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FClientExists(fClient.CId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CFkBirthCityId"] = new SelectList(_context.FCities, "CiId", "CiId", fClient.CFkBirthCityId);
            ViewData["CFkCityId"] = new SelectList(_context.FCities, "CiId", "CiId", fClient.CFkCityId);
            ViewData["CFkMediaId"] = new SelectList(_context.FCategories, "CaId", "CaId", fClient.CFkMediaId);
            return View(fClient);
        }

        // GET: FClients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fClient = await _context.FClients
                .Include(f => f.CFkBirthCity)
                .Include(f => f.CFkCity)
                .Include(f => f.CFkMedia)
                .FirstOrDefaultAsync(m => m.CId == id);
            if (fClient == null)
            {
                return NotFound();
            }

            return View(fClient);
        }

        // POST: FClients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fClient = await _context.FClients.FindAsync(id);
            _context.FClients.Remove(fClient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FClientExists(int id)
        {
            return _context.FClients.Any(e => e.CId == id);
        }
        public IActionResult GetMatchingCities(string input)
        {
            return Json(FClientUtility.GetMatchingCities(input));
        }

        public IActionResult GenerateTaxCertificates()
        {
            List<SelectListItem> clientsWithNull = _context.FInterventions
                .Where(i => i.IFkInvoice != null)
                .Select(c => new SelectListItem
                {
                    Text = c.IFkClient.CName,
                    Value = c.IFkClient.CId.ToString()
                })
                .ToList();
            clientsWithNull.Add(new SelectListItem { Text = "Select a client", Value = "", Selected = true });


            List<SelectListItem> yearsWithNull = _context.FInvoices
                .Select(i => i.InInvoiceDate.Year.ToString()).Distinct()
                .Select(c => new SelectListItem
                {
                    Text = c.ToString(),
                    Value = c.ToString(),
                })
                .ToList();
            yearsWithNull.Add(new SelectListItem { Text = "Select a year", Value = "", Selected = true });



            ViewData["IFkInvoice"] = new SelectList(clientsWithNull, "Value", "Text", "");
            ViewData["Year"] = new SelectList(yearsWithNull, "Value", "Text", "");


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateTaxCertificates(int clientId, int civilYear)
        {
            if (ModelState.IsValid)
            {
                var client = _context.FClients.Include(f => f.CFkCity).Where(c=>c.CId==clientId).First();

                if (client == null)
                {
                    return NotFound();
                }

                var pdfBytes = GenerateInvoicePdf(client, civilYear);

                return File(pdfBytes, "application/pdf", $"summary_{civilYear}_invoices_for_client_{client.CName}_no_{client.CId}.pdf");
            }
            return View();
        }
        private byte[] GenerateInvoicePdf(FClient client, int civilYear)
        {
            var invoices = _context.FInterventions
                .Where(i=>i.IFkClient == client && i.IFkInvoice != null && i.IFkInvoice.InInvoiceDate.Year == civilYear)
                .Select(i=>i.IFkInvoice)
                .ToList();

            var htmlContent = GetHtmlContent(client, invoices);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
                },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = htmlContent
                    }
                }
            };

            return _converter.Convert(doc);
        }

        private string GetHtmlContent(FClient client, List<FInvoice> invoices)
        {
            var html = @"
                <h1>Client Information</h1>
                <p><strong>c_id:</strong> @Model.ClientId</p>
                <p><strong>c_name:</strong> @Model.ClientName</p>
                <p><strong>ci_name:</strong> @Model.CityName</p>
                <h1>Invoices</h1>
                <table>
                    <tr>
                        <th>in_invoice_id</th>
                        <th>in_invoice_date</th>
                        <th>in_amount</th>
                    </tr>
                    @foreach (var invoice in Model.Invoices)
                    {
                        <tr>
                            <td>@invoice.InInvoiceId</td>
                            <td>@invoice.InInvoiceDate</td>
                            <td>@invoice.InAmount</td>
                        </tr>
                    }
                </table>
            ";

            html = html.Replace("@Model.ClientId", client.CId.ToString());
            html = html.Replace("@Model.ClientName", client.CName);
            html = html.Replace("@Model.CityName", client.CFkCity.CiName);

            var invoiceRows = "";
            foreach (var invoice in invoices)
            {
                invoiceRows += $@"
                    <tr>
                        <td>{invoice.InInvoiceId}</td>
                        <td>{invoice.InInvoiceDate}</td>
                        <td>{invoice.InAmount}</td>
                    </tr>
                ";
            }

            html = html.Replace("@foreach (var invoice in Model.Invoices)", invoiceRows);

            return html;
        }
    }

}
