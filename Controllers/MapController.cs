using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;

namespace LibrarySystem.Controllers
{
    public class MapController : Controller
    {
        private readonly KütüphaneeContext _context;

        public MapController(KütüphaneeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Veritabanından sadece haritada gösterilebilecek (Koordinatı olan) şubeleri çekiyoruz.
            var branches = await _context.LibraryBranches
                                         .Where(b => b.Location != null)
                                         .ToListAsync();

            return View(branches);
        }
    }
}