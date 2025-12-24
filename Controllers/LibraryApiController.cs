using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using NetTopologySuite.Geometries;

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

        // ==========================================
        // 🌍 1. MEKANSAL KAYNAK: ŞUBELER
        // ==========================================

        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _context.LibraryBranches
                .Select(b => new 
                {
                    b.Id,
                    b.Name,
                    b.Address,
                    Lat = b.Location.Y, 
                    Lng = b.Location.X  
                })
                .ToListAsync();

            return Ok(branches);
        }

        [HttpPost("branches")]
        [Authorize(Roles = "admin")] 
        public async Task<IActionResult> AddBranch([FromBody] BranchDto data)
        {
            if (data == null || string.IsNullOrEmpty(data.Name))
                return BadRequest("Şube adı boş olamaz.");

            var location = new Point(data.Lng, data.Lat) { SRID = 4326 };

            var newBranch = new LibraryBranch
            {
                Name = data.Name,
                Address = data.Address,
                Location = location
            };

            _context.LibraryBranches.Add(newBranch);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Şube başarıyla eklendi!" });
        }

        [HttpDelete("branches/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.LibraryBranches.FindAsync(id);
            if (branch == null) return NotFound("Kütüphane bulunamadı.");

            _context.LibraryBranches.Remove(branch);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kütüphane başarıyla silindi." });
        }

// ==========================================
        // 🔄 GÜNCELLEME (PUT) İŞLEMİ (EKSİK OLAN PARÇA)
        // ==========================================
        
        // PUT: api/LibraryApi/branches/5
        [HttpPut("branches/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] BranchDto data)
        {
            // 1. Güncellenecek şubeyi bul
            var branch = await _context.LibraryBranches.FindAsync(id);
            if (branch == null) return NotFound("Şube bulunamadı.");

            // 2. İsim ve Adres bilgilerini güncelle (Attributes)
            branch.Name = data.Name;
            branch.Address = data.Address;

            // 3. Eğer koordinat gönderildiyse harita konumunu güncelle (Geometry)
            // (0 değilse yeni konum var demektir)
            if (data.Lat != 0 && data.Lng != 0)
            {
                branch.Location = new Point(data.Lng, data.Lat) { SRID = 4326 };
            }

            // 4. Kaydet
            await _context.SaveChangesAsync();

            return Ok(new { message = "Şube bilgileri ve konumu güncellendi." });
        }



        // ==========================================
        // 📚 2. MEKANSAL OLMAYAN KAYNAK: KİTAPLAR
        // ==========================================

        [HttpGet("books")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _context.Books
                .Select(b => new 
                {
                    b.BookId,
                    b.Title,
                    b.Author,
                    // Veritabanındaki int? (nullable) tipini normal int'e çeviriyoruz
                    Stock = b.TotalStock ?? 0 
                })
                .ToListAsync();

            return Ok(books);
        }

        [HttpPost("books")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBook([FromBody] BookDto bookData)
        {
            if (bookData == null || string.IsNullOrEmpty(bookData.Title))
                return BadRequest("Kitap adı boş olamaz.");

            var newBook = new Book
            {
                Title = bookData.Title,
                Author = bookData.Author,
                Category = "Genel", // Varsayılan kategori
                
                // 👇 Senin Modelindeki İsimler:
                TotalStock = bookData.Stock,   // Toplam Stok
                CurrentStock = bookData.Stock, // Mevcut Stok (Başlangıçta eşittir)
                
                // 👇 Yeni eklendiği için bugünün tarihini atıyoruz
                DateAdded = DateTime.Now 

                // ❌ Image satırını SİLDİM çünkü modelinde yok.
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kitap API üzerinden eklendi." });
        }
    }

    // ==========================================
    // 📦 DTO MODELLERİ
    // ==========================================

    public class BranchDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class BookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Stock { get; set; }
    }
}