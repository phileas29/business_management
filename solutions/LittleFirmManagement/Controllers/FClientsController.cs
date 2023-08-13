using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;

namespace LittleFirmManagement.Controllers
{
    public class FClientsController : Controller
    {
        private readonly FirmContext _context;
        private readonly IConverter _converter;
        IWebHostEnvironment _webHostEnvironment;

        public FClientsController(FirmContext context, IConverter converter, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _converter = converter;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: FClients
        public async Task<IActionResult> Index(FClientIndexViewModel model)
        {
            var clients = _context.FClients
                .OrderBy(c=>c.CName)
                .Include(f => f.CFkCity)
                .AsQueryable();

            if (!string.IsNullOrEmpty(model.NameSearch))
                clients = clients.Where(c => c.CName.ToLower().Contains(model.NameSearch.ToLower()));
                //clients = clients.Where(c => -1 < c.CName.IndexOf(model.NameSearch, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(model.FirstnameSearch))
                clients = clients.Where(c => c.CFirstname.ToLower().Contains(model.FirstnameSearch.ToLower()));

            if (0 < model.CitySearch)
                clients = clients.Where(c => c.CFkCity.CiId == model.CitySearch);

            // Calculate pagination values
            int totalClients = clients.Count();
            int totalPages = (int)Math.Ceiling((double)totalClients / model.PageSize);

            // Apply pagination
            var clientsSelectedPage = clients.Skip((model.Page - 1) * model.PageSize).Take(model.PageSize);

            // Pass the paginated clients and pagination data to the view
            if (model.NameSearch == null && model.FirstnameSearch == null && model.CitySearch == null)
                model.ClientsGPS = null;
            else
                model.ClientsGPS = clients.Except(clientsSelectedPage);

            model.Clients = clientsSelectedPage.ToList();
            model.TotalClients = totalClients;
            model.TotalPages = totalPages;

            List<SelectListItem> citiesWithNull = _context.FClients
                .Select(c => new SelectListItem
                {
                    Text = c.CFkCity.CiName,
                    Value = c.CFkCityId.ToString()
                })
                .Distinct()
                .OrderBy(c => c.Text)
                .ToList();
            citiesWithNull.Insert(0,new SelectListItem { Text = "Select a city", Value = "", Selected = true });
            model.Cities = new SelectList(citiesWithNull, "Value", "Text", "");

            return View(model);
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

        private void PrepareViewData(FClientCreateViewModel model)
        {
            List<SelectListItem> mediasWithNull = _context.FCategories
                .Where(c => c.CaFkCategoryType.CtName == "m�dia")
                .Select(c => new SelectListItem
                {
                    Text = c.CaName,
                    Value = c.CaId.ToString()
                })
                .ToList();
            mediasWithNull.Add(new SelectListItem { Text = "Select a media", Value = "", Selected = true });
            model.Medias = new SelectList(mediasWithNull, "Value", "Text", "");
        }

        // GET: FClients/Create
        public IActionResult Create()
        {
            FClientCreateViewModel model = new();

            PrepareViewData(model);
            //var viewModel = new FClientsViewModel
            //{
            //    CName = "Default Name",
            //    CFirstname = "Default Firstname",
            //    CEmail = "default@example.com",
            //    CPhoneFixed = "1234567890",
            //    CPhoneCell = "9876543210",
            //    CFkMediaId = _context.FCategories
            //    .Where(c => c.CaFkCategoryType.CtName == "m�dia" && c.CaName == "journal").Select(m => m.CaId).First()
            //};
            return View(model);
        }

        // POST: FClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CFkMediaId,CName,CFirstname,CAddress,CEmail,CPhoneFixed,CPhoneCell,CIsPro,CLocationLong,CLocationLat,CDistance,CTravelTime,CUrssafUuid,CIsMan,CBirthName,CBirthCountryCode,CBirthDate,CBic,CIban,CAccountHolder,Town,BirthCityInput")] FClientCreateViewModel model, bool saveAndExit)
        {
            if (ModelState.IsValid)
            {
                FClient fClient = FClientUtility.ValidateClient(model, _context).Result;
                _context.Add(fClient);
                await _context.SaveChangesAsync();
                if (saveAndExit)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Create", "FInterventions", new { id = fClient.CId });
            }
            PrepareViewData(model);
            return View(model);
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
        public void PrepareViewDataGenerateTaxCertificates(FClientGenerateTaxCertificatesViewModel model)
        {
            List<SelectListItem> yearsWithNull = _context.FInvoices
                .Select(i => i.InInvoiceDate.Year.ToString())
                .Distinct()
                .Select(c => new SelectListItem
                {
                    Text = c.ToString(),
                    Value = c.ToString()
                })
                .ToList();
            yearsWithNull.Insert(0, new SelectListItem { Text = "Select a year", Value = "", Selected = true });

            model.CivilYear = new SelectList(yearsWithNull, "Value", "Text", "");

        }

        public IActionResult GenerateTaxCertificates()
        {
            FClientGenerateTaxCertificatesViewModel model = new();

            PrepareViewDataGenerateTaxCertificates(model);

            return View(model);
        }


        [HttpPost]
        public IActionResult GenerateTaxCertificates(FClientGenerateTaxCertificatesViewModel model)
        {
            int civilYear = model.CivilYearId;
            if (ModelState.IsValid)
            {
                List<FClient> clients = _context.FInterventions
                    .Include(i => i.IFkClient.CFkCity)
                    .Where(i=>i.IFkInvoice!=null&&i.IFkInvoice.InInvoiceDate.Year==civilYear)
                    .Select(i=>i.IFkClient)
                    .ToList();

                foreach (FClient client in clients)
                {
                    var doc = GenerateInvoicePdf(client, civilYear);
                    string dest = $@"{_webHostEnvironment.WebRootPath}\\pdf\\attestation_fiscale_{civilYear}_PHILEAS_INFORMATIQUE_de_M-ME_{client.CName}_{client.CFirstname}.pdf";

                    System.IO.File.WriteAllBytes(dest, doc);


                    //doc.SaveAs(dest);

                    //using (FileStream pdfDest = new FileStream(dest, FileMode.Create))
                    //{
                    //    ConverterProperties converterProperties = new ConverterProperties();
                    //    HtmlConverter.ConvertToPdf(doc, pdfDest, converterProperties);
                    //}
                }
                return RedirectToAction("Index", "Home");
            }
            PrepareViewDataGenerateTaxCertificates(model);
            return View(model);
        }
        private byte[] GenerateInvoicePdf(FClient client, int civilYear)
        {
            List<FClientGenerateTaxCertificatesBusinessModel> invoices = _context.FInterventions
                .Where(i => i.IFkClient == client && i.IFkInvoice != null && i.IFkInvoice.InInvoiceDate.Year == civilYear)
                .Select(i => 
                new FClientGenerateTaxCertificatesBusinessModel
                {
                    Date = i.IFkInvoice.InInvoiceDate,
                    Duration = Math.Round(i.IFkCategory.CaName == "assistance informatique" ? i.IFkInvoice.InAmount / 60m : i.IFkInvoice.InAmount / 40m,2),
                    HourlyRate = i.IFkCategory.CaName == "assistance informatique" ? 60 : 40,
                    Amount = i.IFkInvoice.InAmount
                })
                .ToList();

            var htmlContent = GetHtmlContent(client, invoices, civilYear);


            //var renderer = new HtmlToPdf();
            //var res = renderer.RenderHtmlAsPdf(htmlContent);
            //return res;

            //return htmlContent;

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

        private string GetHtmlContent(FClient client, List<FClientGenerateTaxCertificatesBusinessModel> invoices, int civilYear)
        {
            var html = @"<!DOCTYPE html>
                <html lang=""en"">

                <head>
                    <meta charset=""UTF-8"">
                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Attestation fiscale _YYYY PHILEAS INFORMATIQUE de M-ME NAME FIRSTNAME</title>
                    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css"" rel=""stylesheet""
                        integrity=""sha384-rbsA2VBKQhggwzxH7pPCaAqO46MgnOM80zW1RWuH61DGLwZJEdK2Kadq2F9CUG65"" crossorigin=""anonymous"">
                </head>

                <body>
                    <div class=""container"">
                        <div class=""row"" style=""height: 150px;"">
                            <div class=""col-6 h-100"">
                                <img class=""img-fluid h-100"" src=""_IMG_PATH/logo_.jpg"" alt="""">
                            </div>
                            <div class=""col-6"">
                                <h1>Attestation fiscale portant sur les d�penses de _YYYY</h1>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col-6 d-flex flex-column"">
                                <p class=""mb-0"">�metteur :</p>
                                <div class=""bg-secondary p-3 text-white h-100"">
                                    <p class=""m-0"">Phil�as informatique</p>
                                    <p class=""m-0"">29 chemin de lesquidic-nevez</p>
                                    <p class=""mb-2"">29950 GOUESNAC'H</p>
                                    <p class=""m-0"">T�l.: 07 66 62 44 85</p>
                                    <p class=""m-0"">Email: contact@phileasinformatique.fr</p>
                                    <p class=""m-0"">Web: https://phileasinformatique.fr/</p>
                                    <p class=""m-0"">Siret: 881 585 939 00013</p>
                                </div>
                            </div>
                            <div class=""col-6 d-flex flex-column"">
                                <p class=""mb-0"">Adress� � :</p>
                                <div class=""border border-secondary p-3 h-100"">
                                    <p class=""m-0"">_NAME _FIRSTNAME</p>
                                    <p class=""m-0"">_ADDRESS</p>
                                    <p class=""mb-2"">_CITY</p>
                                </div>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col"">
                                <p>Je soussign�, M. PERON Phil�as, dirigeant de la micro-entreprise � Phil�as informatique �, certifie
                                    que M./Mme _NAME _FIRSTNAME a b�n�fici� d'une intervention � domicile.</p>
                                <p>Montant total des interventions effectivement acquitt�es ouvrant droit � cr�dit d'imp�t :
                                    _TOTAL_AMOUNT</p>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col"">
                                <p class=""mb-0"">Montants exprim�s en Euros</p>
                                <table class=""table table-bordered border border-5"">
                                    <thead>
                                        <tr>
                                            <th scope=""col"">Date</th>
                                            <th scope=""col"">Intervenant</th>
                                            <th scope=""col"">Dur�e</th>
                                            <th scope=""col"">Taux horaire</th>
                                            <th scope=""col"">Total</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        _INVOICE
                                    </tbody>
                                    <tfoot>
                                        <tr>
                                            <th colspan=""4"" scope=""row"">Montant �ligible _YYYY</th>
                                            <td>_TOTAL_AMOUNT</td>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col"">
                                <p>La d�claration engage la responsabilit� du seul contribuable.</p>
                                <p>Fait pour valoir ce que de droit,</p>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col text-end"">
                                <p>Le 01/01/_+1YYYY</p>
                                <img src=""_IMG_PATH/signature.png"" alt="""">
                            </div>
                        </div>


                        <div class=""row"">
                            <div class=""col"">
                                <p style=""font-size: 0.8rem;text-align: center;"">""Phil�as informatique"" est une micro-entreprise agr��e pour les services � la personne par arr�t�
                                    pr�fectoral n� SAP881585939 en date du 03/11/2020</p>
                            </div>
                        </div>
                    </div>
                </body>

                </html>
            "
            ;

            html = html.Replace("_IMG_PATH", _webHostEnvironment.WebRootPath+"\\img");

            html = html.Replace("_TOTAL_AMOUNT", invoices.Select(i=>i.Amount).Sum().ToString()+ " �");
            html = html.Replace("_NAME", client.CName);
            html = html.Replace("_FIRSTNAME", client.CFirstname);
            html = html.Replace("_ADDRESS", client.CAddress);
            html = html.Replace("_CITY", client.CFkCity.CiPostalCode + " " + client.CFkCity.CiName);

            var invoiceRows = "";
            foreach (var invoice in invoices)
            {
                invoiceRows += $@"
                    <tr>
                        <td>{invoice.Date.ToShortDateString()}</td>
                        <td>Phil�as PERON</td>
                        <td>{invoice.Duration} h</td>
                        <td>{invoice.HourlyRate} �/h</td>
                        <td>{invoice.Amount} �</td>
                    </tr>
                ";
            }

            html = html.Replace("_INVOICE", invoiceRows);

            html = html.Replace("_YYYY", civilYear.ToString());
            html = html.Replace("_+1YYYY", $"{civilYear + 1}");

            return html;
        }
    }

}
