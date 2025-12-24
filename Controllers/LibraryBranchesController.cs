using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;
using NetTopologySuite.Geometries;
using System.Globalization; // ÖNEMLİ: Nokta/Virgül ayarı için şart

namespace LibrarySystem.Controllers
{
    public class LibraryBranchesController : Controller
    {
        private readonly KütüphaneeContext _context;

        public LibraryBranchesController(KütüphaneeContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME SAYFASI
        public async Task<IActionResult> Index()
        {
            return View(await _context.LibraryBranches.ToListAsync());
        }

        // 2. EKLEME SAYFASI (GET)
        // Haritada diğer şubeleri de göstermek için veri gönderiyoruz
        public IActionResult Create()
        {
            var existingBranches = _context.LibraryBranches
                .Where(b => b.Location != null)
                .Select(b => new
                {
                    b.Name,
                    Lat = b.Location.Y,
                    Lng = b.Location.X
                })
                .ToList();

            ViewBag.ExistingBranches = existingBranches;
            return View();
        }

        // 3. EKLEME İŞLEMİ (POST) - GÜVENLİ VERSİYON 🛡️
        // Parametreleri 'string' alıyoruz ki nokta/virgül düzeltmesini kendimiz yapalım.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LibraryBranch libraryBranch, string latitude, string longitude)
        {
            // A. Gelen veriyi temizle (Virgülleri noktaya çevir)
            // Böylece "39,123" de gelse "39.123" de gelse çalışır.
            string latStr = latitude?.Replace(",", ".") ?? "0";
            string lngStr = longitude?.Replace(",", ".") ?? "0";

            double lat, lng;

            // B. Güvenli Çeviri (InvariantCulture: Her zaman noktayı ondalık sayar)
            bool isLatOk = double.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out lat);
            bool isLngOk = double.TryParse(lngStr, NumberStyles.Any, CultureInfo.InvariantCulture, out lng);

            // C. Hata Kontrolü
            if (!isLatOk || !isLngOk || lat == 0 || lng == 0)
            {
                ModelState.AddModelError("", "Koordinatlar hatalı veya seçilmedi. Lütfen haritadan tekrar seçiniz.");
                
                // Hata olursa haritadaki noktalar kaybolmasın diye tekrar yüklüyoruz
                var existingBranches = _context.LibraryBranches
                    .Where(b => b.Location != null)
                    .Select(b => new { b.Name, Lat = b.Location.Y, Lng = b.Location.X })
                    .ToList();
                ViewBag.ExistingBranches = existingBranches;

                return View(libraryBranch);
            }

            // D. Veritabanı Formatına Çevir (Point: Boylam, Enlem)
            // NetTopologySuite -> Point(x, y) yani Point(Boylam, Enlem)
            libraryBranch.Location = new Point(lng, lat) { SRID = 4326 };

            ModelState.Remove("Location"); // Location'ı elle doldurduk, hata vermesin.

            if (ModelState.IsValid)
            {
                _context.Add(libraryBranch);
                await _context.SaveChangesAsync();
                // Başarılı olursa Harita sayfasına git
                return RedirectToAction("Index", "Map");
            }
            
            return View(libraryBranch);
        }

        // 4. DÜZENLEME SAYFASI (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var branch = await _context.LibraryBranches.FindAsync(id);
            if (branch == null) return NotFound();

            return View(branch);
        }

        // 5. DÜZENLEME İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LibraryBranch libraryBranch)
        {
            if (id != libraryBranch.Id) return NotFound();

            // Düzenleme sırasında konum bozulmasın diye eski konumu veritabanından çekip koruyoruz.
            // (Şimdilik sadece İsim/Adres güncelliyoruz)
            var existingBranch = await _context.LibraryBranches
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingBranch != null)
            {
                libraryBranch.Location = existingBranch.Location;
            }
            
            ModelState.Remove("Location");

            if (ModelState.IsValid)
            {
                _context.Update(libraryBranch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(libraryBranch);
        }

        // 6. SİLME ONAY SAYFASI (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var branch = await _context.LibraryBranches.FirstOrDefaultAsync(m => m.Id == id);
            if (branch == null) return NotFound();

            return View(branch);
        }

        // 7. SİLME İŞLEMİ (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _context.LibraryBranches.FindAsync(id);
            if (branch != null)
            {
                _context.LibraryBranches.Remove(branch);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}