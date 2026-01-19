using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System_wymiany_żywności.Models;

namespace System_wymiany_żywności.Controllers
{
    // Podstawowy kontroler obsługujący statyczne strony
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Akcja obsługująca wyświetlanie błędów systemowych
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
