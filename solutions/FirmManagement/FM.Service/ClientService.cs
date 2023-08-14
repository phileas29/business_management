using FM.Domain.Abstractions.Repository;
using FM.Domain.Abstractions.Service;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using FM.Domain.Models;

namespace FM.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IInterventionRepository _interventionRepository;
        private readonly ICityService _cityService;
        private readonly IConverter _converter;

        public ClientService(IClientRepository clientRepository, ICityRepository cityRepository, ICityService cityService, IConverter converter, IInterventionRepository interventionRepository)
        {
            _clientRepository = clientRepository;
            _cityRepository = cityRepository;
            _cityService = cityService;
            _converter = converter;
            _interventionRepository = interventionRepository;
        }

        public async Task<FClient> GetRepositoryClientFromWebModelAsync(ClientCreateWebModel wClient)
        {
            List<string?> jCities = new() { wClient.Town, wClient.BirthCityInput };
            int[] cFkCities = new int[] { 0, 0 };

            for(int i=0; i<2;i++)
            {
                if (jCities[i] != null)
                {
                    CityJsonRepositoryModel? jCity = _cityRepository.SelectCityByNameFromFranceJsonDb(jCities[i]!);
                    if (jCity != null)
                    {
                        FCity? fCity = await _cityRepository.SelectCityByCodeAsync(int.Parse(jCity.Code!));
                        if (fCity == null)
                        {
                            await _cityRepository.InsertCityAsync(
                                new FCity
                                {
                                    CiPostalCode = jCity.CodesPostaux![0],
                                    CiName = jCity.Nom!.ToUpper(),
                                    CiInseeCode = int.Parse(jCity.Code!),
                                    CiDepartCode = int.Parse(jCity.CodeDepartement!)
                                });
                        }
                        else
                            cFkCities[i] = fCity.CiId;
                    }
                }
            }

            wClient.CFkCityId = cFkCities[0];
            wClient.CFkBirthCityId = cFkCities[1] == 0 ? null : cFkCities[1];

            var fClient = new FClient
            {
                CFkMediaId = wClient.CFkMediaId,
                CFkCityId = wClient.CFkCityId,
                CFkBirthCityId = wClient.CFkBirthCityId,
                CName = wClient.CName,
                CFirstname = wClient.CFirstname,
                CAddress = wClient.CAddress,
                CEmail = wClient.CEmail,
                CPhoneFixed = wClient.CPhoneFixed,
                CPhoneCell = wClient.CPhoneCell,
                CIsPro = wClient.CIsPro,
                CLocationLong = wClient.CLocationLong,
                CLocationLat = wClient.CLocationLat,
                CDistance = wClient.CDistance,
                CTravelTime = wClient.CTravelTime,
                CUrssafUuid = wClient.CUrssafUuid,
                CIsMan = wClient.CIsMan,
                CBirthName = wClient.CBirthName,
                CBirthDate = wClient.CBirthDate,
                CBic = wClient.CBic,
                CIban = wClient.CIban,
                CAccountHolder = wClient.CAccountHolder
            };

            return fClient;
        }

        public async Task<int> PutClientAsync(FClient fClient)
        {
            return await _clientRepository.InsertClientAsync(fClient);
        }

        public async Task<List<FClient>> GetAllClientsAsync()
        {
            return await _clientRepository.SelectAllClientsAsync();
        }

        public async Task<ClientIndexWebModel> GetClientIndexWebModelAsync(ClientIndexWebModel wClient)
        {
            List<FClient> selectedClients = await _clientRepository.SelectClientsByNameOrFirstnameOrCityAsync(wClient.NameSearch,wClient.FirstnameSearch,wClient.CitySearch,wClient.PageSize,wClient.Page);
            wClient.Clients = selectedClients.Skip((wClient.Page - 1) * wClient.PageSize).Take(wClient.PageSize).ToList();
            if (wClient.NameSearch == null && wClient.FirstnameSearch == null && wClient.CitySearch == null)
                wClient.ClientsGPS = null;
            else
                wClient.ClientsGPS = selectedClients.Except(wClient.Clients).ToList();
            wClient.Cities = await _cityService.GetSelectListAsync();
            int totalClients = selectedClients.Count();
            int totalPages = (int)Math.Ceiling((double)totalClients / wClient.PageSize);
            wClient.TotalClients = totalClients;
            wClient.TotalPages = totalPages;
            return wClient;
        }

        public async Task<bool> GenerateTaxCertificates(ClientGenerateTaxCertificatesWebModel wClient, string webRootPath)
        {
            List<FClient> clients = await _clientRepository.SelectAllInvoicedClientsByYear(wClient.CivilYearId);
            foreach (FClient client in clients)
            {
                byte[] doc = await GenerateInvoicePdfAsync(client, wClient.CivilYearId, webRootPath);
                string dest = $@"{webRootPath}\\pdf\\attestation_fiscale_{wClient.CivilYearId}_PHILEAS_INFORMATIQUE_de_M-ME_{client.CName}_{client.CFirstname}.pdf";

                File.WriteAllBytes(dest, doc);
            }
            return true;
        }

        private async Task<byte[]> GenerateInvoicePdfAsync(FClient client, int civilYearId, string webRootPath)
        {
            List<FIntervention> interventions = await _interventionRepository
                .SelectAllInvoicedInterventionsByYearAndByClient(civilYearId, client);
            List<ClientGenerateTaxCertificatesServiceModel> invoices = interventions
                .Select(i =>
                new ClientGenerateTaxCertificatesServiceModel
                {
                    Date = i.IFkInvoice!.InInvoiceDate,
                    Duration = Math.Round(i.IFkInvoice.InAmount / ( i.IFkCategory.CaName == "assistance informatique" ? 60m : 40m ) / i.IFkInvoice.InAmount, 2),
                    HourlyRate = i.IFkCategory.CaName == "assistance informatique" ? 60 : 40,
                    Amount = i.IFkInvoice.InAmount
                })
                .ToList();
            string htmlContent = GetHtmlContent(client, invoices, civilYearId, webRootPath);

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

        private static string GetHtmlContent(FClient client, List<ClientGenerateTaxCertificatesServiceModel> invoices, int civilYearId, string webRootPath)
        {
            string html = @"<!DOCTYPE html>
                <html lang=""en"">

                <head>
                    <meta charset=""UTF-8"">
                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Attestation fiscale _YYYY_ PHILEAS INFORMATIQUE de M-ME _NAME_ _FIRSTNAME_</title>
                    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css"" rel=""stylesheet""
                        integrity=""sha384-rbsA2VBKQhggwzxH7pPCaAqO46MgnOM80zW1RWuH61DGLwZJEdK2Kadq2F9CUG65"" crossorigin=""anonymous"">
                </head>

                <body>
                    <div class=""container"">
                        <div class=""row"" style=""height: 150px;"">
                            <div class=""col-6 h-100"">
                                <img class=""img-fluid h-100"" src=""_IMG_PATH_/logo_.jpg"" alt="""">
                            </div>
                            <div class=""col-6"">
                                <h1>Attestation fiscale portant sur les dépenses de _YYYY_</h1>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col-6 d-flex flex-column"">
                                <p class=""mb-0"">Émetteur :</p>
                                <div class=""bg-secondary p-3 text-white h-100"">
                                    <p class=""m-0"">Philéas informatique</p>
                                    <p class=""m-0"">29 chemin de lesquidic-nevez</p>
                                    <p class=""mb-2"">29950 GOUESNAC'H</p>
                                    <p class=""m-0"">Tél.: 07 66 62 44 85</p>
                                    <p class=""m-0"">Email: contact@phileasinformatique.fr</p>
                                    <p class=""m-0"">Web: https://phileasinformatique.fr/</p>
                                    <p class=""m-0"">Siret: 881 585 939 00013</p>
                                </div>
                            </div>
                            <div class=""col-6 d-flex flex-column"">
                                <p class=""mb-0"">Adressé à :</p>
                                <div class=""border border-secondary p-3 h-100"">
                                    <p class=""m-0"">_NAME_ _FIRSTNAME_</p>
                                    <p class=""m-0"">_ADDRESS_</p>
                                    <p class=""mb-2"">_CITY_</p>
                                </div>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col"">
                                <p>Je soussigné, M. PERON Philéas, dirigeant de la micro-entreprise « Philéas informatique », certifie
                                    que M./Mme _NAME_ _FIRSTNAME_ a bénéficié d'une intervention à domicile.</p>
                                <p>Montant total des interventions effectivement acquittées ouvrant droit à crédit d'impôt :
                                    _TOTAL_AMOUNT_</p>
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col"">
                                <p class=""mb-0"">Montants exprimés en Euros</p>
                                <table class=""table table-bordered border border-5"">
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
                                        _INVOICE_
                                    </tbody>
                                    <tfoot>
                                        <tr>
                                            <th colspan=""4"" scope=""row"">Montant éligible _YYYY_</th>
                                            <td>_TOTAL_AMOUNT_</td>
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
                                <p>Le 01/01/_YYYY+1_</p>
                                <img src=""_IMG_PATH_/signature.png"" alt="""">
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

            html = html.Replace("_IMG_PATH_", webRootPath + "\\img");

            html = html.Replace("_TOTAL_AMOUNT_", invoices.Select(i => i.Amount).Sum().ToString() + " €");
            html = html.Replace("_NAME_", client.CName);
            html = html.Replace("_FIRSTNAME_", client.CFirstname);
            html = html.Replace("_ADDRESS_", client.CAddress);
            html = html.Replace("_CITY_", client.CFkCity.CiPostalCode + " " + client.CFkCity.CiName);

            string invoiceRows = "";
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

            html = html.Replace("_INVOICE_", invoiceRows);

            html = html.Replace("_YYYY_", civilYearId.ToString());
            html = html.Replace("_YYYY+1_", $"{civilYearId + 1}");

            return html;
        }
    }
}
