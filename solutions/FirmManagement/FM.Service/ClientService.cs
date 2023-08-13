using FM.Domain.Abstractions.Repository;
using FM.Domain.Abstractions.Service;
using FM.Domain.Models.Repository;
using FM.Domain.Models.Web;

namespace FM.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ICityService _cityService;
        public ClientService(IClientRepository clientRepository, ICityRepository cityRepository, ICityService cityService)
        {
            _clientRepository = clientRepository;
            _cityRepository = cityRepository;
            _cityService = cityService;
        }

        public async Task<FClient> GetRepositoryClientFromWebModelAsync(ClientCreateWebModel wClient)
        {
            List<string?> jCities = new List<string?>() { wClient.Town, wClient.BirthCityInput };
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
    }
}
