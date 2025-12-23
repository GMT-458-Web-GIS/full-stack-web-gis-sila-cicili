using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;

namespace LibrarySystem.Controllers;

public class HomeController : Controller
{
    private readonly KütüphaneeContext _context;

    // Veritabanı bağlantısını buraya da ekledik
    public HomeController(KütüphaneeContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // İstatistikleri veritabanından çekip ViewBag (Çanta) içine atıyoruz
        ViewBag.TotalBooks = _context.Books.Count();
        ViewBag.TotalUsers = _context.Users.Count();
        ViewBag.ActiveLoans = _context.Loans.Where(l => l.Status == "active").Count();

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