using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using LibrarySystem.Services; // Unutma

namespace LibrarySystem.Controllers
{
    public class HomeController : Controller
    {
        // Context GİTTİ, Servis GELDİ
        private readonly IDashboardService _dashboardService;

        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            // Verileri servisten isteyip ViewBag'e koyuyoruz
            ViewBag.TotalBooks = await _dashboardService.ToplamKitapSayisi();
            ViewBag.TotalUsers = await _dashboardService.ToplamUyeSayisi();
            ViewBag.ActiveLoans = await _dashboardService.AktifOduncSayisi();

            return View();
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