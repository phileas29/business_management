using AddressFinder.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace AddressFinder.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new AddressViewModel());
        }
        [HttpPost]
        public AddressViewModel FindAddress(AddressViewModel model)
        {
            var client = new RestClient("https://nominatim.openstreetmap.org");
            var request = new RestRequest("search");
            request.AddParameter("q", $"{model.Address}, {model.Town}");
            request.AddParameter("format", "json");
            request.AddParameter("limit", 1);

            var response = client.Get(request);

            var content = response.Content;

            var results = JsonConvert.DeserializeObject<OpenStreetMapResult[]>(content);

            if (results != null && results.Length > 0)
            {
                var result = results[0];
                model.Address = result.Display_Name;
                model.Latitude = result.Lat;
                model.Longitude = result.Lon;
            }
            else
            {
                ModelState.AddModelError("", "Unable to find address");
            }

            return model;
        }
    }
}
