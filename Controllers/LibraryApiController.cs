using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryApiController : ControllerBase
    {
        private readonly KütüphaneeContext _context;

        public LibraryApiController(KütüphaneeContext context)
        {
            _context = context;
        }

        // GET: api/LibraryApi
        // Bu adres çağrıldığında kütüphanelerin koordinatlarını JSON olarak döner.
        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _context.LibraryBranches
                .Select(b => new 
                {
                    b.Id,
                    b.Name,
                    b.Address,
                    // Koordinatları X (Boylam) ve Y (Enlem) olarak ayırıp gönderiyoruz
                    Lat = b.Location.Y,
                    Lng = b.Location.X
                })
                .ToListAsync();

            return Ok(branches);
        }
    }
}
