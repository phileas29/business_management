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
                    Duration = Math.Round(i.IFkInvoice.InAmount / ( i.IFkCategory.CaName == "assistance informatique" ? 60m : 40m ), 2),
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
            string html = File.ReadAllText("tax_certificate_model.html");

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
                        <td class=""text-end"">{invoice.Duration} h</td>
                        <td class=""text-end"">{invoice.HourlyRate} €/h</td>
                        <td class=""text-end"">{invoice.Amount} €</td>
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
