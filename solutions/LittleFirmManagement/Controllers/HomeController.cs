using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LittleFirmManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleFirmManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FirmContext _context;

    public HomeController(ILogger<HomeController> logger, FirmContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.FClients.ToListAsync());
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
