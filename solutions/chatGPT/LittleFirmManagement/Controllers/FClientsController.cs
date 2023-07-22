using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LittleFirmManagement.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Build.Framework;

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
                .Select(c => c.IFkClient) // Flatten the collection of IFkClient objects
                .Select(c => new SelectListItem
                {
                    Text = c.CName + " " + c.CFirstname,
                    Value = c.CId.ToString()
                })
                .Distinct()
                .OrderBy(i => i.Text)
                .ToList();
            clientsWithNull.Insert(0,new SelectListItem { Text = "Select a client", Value = "", Selected = true });


            List<SelectListItem> yearsWithNull = _context.FInvoices
                .Select(i => i.InInvoiceDate.Year.ToString())
                .Distinct()
                .Select(c => new SelectListItem
                {
                    Text = c.ToString(),
                    Value = c.ToString(),
                })
                .ToList();
            yearsWithNull.Insert(0, new SelectListItem { Text = "Select a year", Value = "", Selected = true });



            ViewData["IFkInvoice"] = new SelectList(clientsWithNull, "Value", "Text", "");
            ViewData["Year"] = new SelectList(yearsWithNull, "Value", "Text", "");


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateTaxCertificates(int civilYear)
        {
            if (ModelState.IsValid)
            {
                List<FClient> clients = _context.FInterventions.Include(i => i.IFkClient.CFkCity).Where(i=>i.IFkInvoice!=null&&i.IFkInvoice.InInvoiceDate.Year==civilYear).Select(i=>i.IFkClient).ToList();

                foreach (FClient client in clients)
                {
                    var pdfBytes = GenerateInvoicePdf(client, civilYear);
                    System.IO.File.WriteAllBytes($@"{_webHostEnvironment.WebRootPath}\\pdf\\attestation_fiscale_{civilYear}_PHILEAS_INFORMATIQUE_de_M-ME_{client.CName}_{client.CFirstname}.pdf", pdfBytes);
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public class InvoiceViewModel
        {
            public DateTime Date { get; set; }
            public decimal Duration { get; set; }
            public int HourlyRate { get; set; }
            public int Amount { get; set; }
        }
        private byte[] GenerateInvoicePdf(FClient client, int civilYear)
        {
            List<InvoiceViewModel> invoices = _context.FInterventions
                .Where(i => i.IFkClient == client && i.IFkInvoice != null && i.IFkInvoice.InInvoiceDate.Year == civilYear)
                .Select(i => 
                new InvoiceViewModel
                {
                    Date = i.IFkInvoice.InInvoiceDate,
                    Duration = Math.Round(i.IFkCategory.CaName == "assistance informatique" ? i.IFkInvoice.InAmount / 60m : i.IFkInvoice.InAmount / 40m,2),
                    HourlyRate = i.IFkCategory.CaName == "assistance informatique" ? 60 : 40,
                    Amount = i.IFkInvoice.InAmount
                })
                .ToList();

            var htmlContent = GetHtmlContent(client, invoices, civilYear);

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

        private string GetHtmlContent(FClient client, List<InvoiceViewModel> invoices, int civilYear)
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
                    <style>
                        /* Custom CSS */
                        body-custom {
                            font-family: Arial, sans-serif;
                            margin: 0;
                            padding: 0;
                        }

                        .container-custom {
                            width: 100%;
                            max-width: 1200px;
                            margin: 0 auto;
                            padding: 10px;
                        }

                        .row-custom {
                            margin: -10px; /* Negative margin to offset padding from columns */
                            overflow: hidden; /* Clearfix for the floated columns */
                        }

                        .col-6-custom {
                            float: left;
                            width: 50%;
                            padding: 10px;
                        }

                        /* Add your custom styles for other classes as needed */

                        /* Example custom styles for specific elements */
                        .h-100-custom {
                            height: 100%;
                        }

                        .img-fluid-custom {
                            max-width: 100%;
                            height: auto;
                        }
                    </style>
                </head>

                <body>
                    <div class=""container-custom"">
                        <div class=""row-custom"">
                            <div class=""col-6-custom h-100-custom"">
                                <img class=""img-fluid-custom h-100-custom"" src=""_IMG_PATH/logo_.jpg"" alt="""">
                            </div>
                            <div class=""col-6-custom"">
                                <h1>Attestation fiscale portant sur les dépenses de _YYYY</h1>
                            </div>
                        </div>
                        <div class=""row-custom"">
                            <div class=""col-6-custom d-flex"">
                                <p class=""mb-0"">Émetteur :</p>
                                <div class=""bg-secondary p-3 text-white h-100-custom"">
                                    <p class=""m-0"">Philéas informatique</p>
                                    <p class=""m-0"">29 chemin de lesquidic-nevez</p>
                                    <p class=""mb-2"">29950 GOUESNAC'H</p>
                                    <p class=""m-0"">Tél.: 07 66 62 44 85</p>
                                    <p class=""m-0"">Email: contact@phileasinformatique.fr</p>
                                    <p class=""m-0"">Web: https://phileasinformatique.fr/</p>
                                    <p class=""m-0"">Siret: 881 585 939 00013</p>
                                </div>
                            </div>
                            <div class=""col-6-custom d-flex"">
                                <p class=""mb-0"">Adressé à :</p>
                                <div class=""border border-secondary p-3 h-100-custom"">
                                    <p class=""m-0"">_NAME _FIRSTNAME</p>
                                    <p class=""m-0"">_ADDRESS</p>
                                    <p class=""mb-2"">_CITY</p>
                                </div>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col"">
                                <p>Je soussigné, M. PERON Philéas, dirigeant de la micro-entreprise « Philéas informatique », certifie
                                    que M./Mme _NAME _FIRSTNAME a bénéficié d'une intervention à domicile.</p>
                                <p>Montant total des interventions effectivement acquittées ouvrant droit à crédit d'impôt :
                                    _TOTAL_AMOUNT</p>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col"">
                                <p class=""mb-0"">Montants exprimés en Euros</p>
                                <table class=""table table-bordered"">
                                    <thead>
                                        <tr>
                                            <th scope=""col"">Date</th>
                                            <th scope=""col"">Intervenant</th>
                                            <th scope=""col"">Durée</th>
                                            <th scope=""col"">Taux horaire</th>
                                            <th scope=""col"">Total</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        _INVOICE
                                    </tbody>
                                    <tfoot>
                                        <tr>
                                            <th colspan=""4"" scope=""row"">Montant éligible _YYYY</th>
                                            <td>_TOTAL_AMOUNT</td>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col"">
                                <p>La déclaration engage la responsabilité du seul contribuable.</p>
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
                                <p style=""font-size: 0.8rem;text-align: center;"">""Philéas informatique"" est une micro-entreprise agréée pour les services à la personne par arrêté
                                    préfectoral n° SAP881585939 en date du 03/11/2020</p>
                            </div>
                        </div>
                    </div>
                </body>

                </html>
            "
            ;

            html = html.Replace("_IMG_PATH", _webHostEnvironment.WebRootPath+"\\img");

            html = html.Replace("_TOTAL_AMOUNT", invoices.Select(i=>i.Amount).Sum().ToString()+ " €");
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
                        <td>Philéas PERON</td>
                        <td>{invoice.Duration} h</td>
                        <td>{invoice.HourlyRate} €/h</td>
                        <td>{invoice.Amount} €</td>
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
