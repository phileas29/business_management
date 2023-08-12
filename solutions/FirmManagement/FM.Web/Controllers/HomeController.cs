using System.Diagnostics;
using FM.Domain.Abstractions.Service;
using FM.Domain.Models.Web;
using Microsoft.AspNetCore.Mvc;

namespace FM.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClientService _clientService;

        public HomeController(ILogger<HomeController> logger, IClientService clientService)
        {
            _logger = logger;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _clientService.GetAllClientsAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}