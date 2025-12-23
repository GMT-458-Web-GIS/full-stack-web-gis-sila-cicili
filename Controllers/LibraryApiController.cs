using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Yetki kontrolü için
using NetTopologySuite.Geometries;      // Harita (Point) işlemleri için

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
        // Tüm kütüphaneleri haritada göstermek için çeker
        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _context.LibraryBranches
                .Select(b => new 
                {
                    b.Id,
                    b.Name,
                    b.Address,
                    Lat = b.Location.Y, // Y = Enlem (Latitude)
                    Lng = b.Location.X  // X = Boylam (Longitude)
                })
                .ToListAsync();

            return Ok(branches);
        }

        // POST: api/LibraryApi
        // Sadece Adminler yeni şube ekleyebilir
        [HttpPost]
        [Authorize(Roles = "admin")] 
        public async Task<IActionResult> AddBranch([FromBody] BranchDto data)
        {
            // 1. Basit Validasyon
            if (data == null || string.IsNullOrEmpty(data.Name))
                return BadRequest("Şube adı boş olamaz.");

            // 2. Koordinat Dönüşümü (Frontend'den gelen Lat/Lng -> PostGIS Point)
            // SRID 4326 standart GPS koordinat sistemidir.
            var location = new Point(data.Lng, data.Lat) { SRID = 4326 };

            // 3. Yeni Nesneyi Oluştur
            var newBranch = new LibraryBranch
            {
                Name = data.Name,
                Address = data.Address, // 👈 GÜNCELLEME BURADA: Artık adresi de kaydediyoruz
                Location = location
            };

            // 4. Veritabanına Ekle ve Kaydet
            _context.LibraryBranches.Add(newBranch);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Başarıyla eklendi!" });
        }
    }

    // Frontend'den gelen veriyi karşılayan paket (Data Transfer Object)
    public class BranchDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty; // 👈 EKLENEN KISIM
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}