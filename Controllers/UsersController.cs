using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization; // Güvenlik Kütüphanesi

namespace LibrarySystem.Controllers
{
    // 🔥 BU SATIR ÇOK ÖNEMLİ: Tüm sayfayı sadece Admin'e kilitler!
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly KütüphaneeContext _context;

        public UsersController(KütüphaneeContext context)
        {
            _context = context;
        }

        // 1. ÜYE LİSTESİ
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.OrderBy(u => u.UserId).ToListAsync();
            return View(users);
        }

        // 2. YENİ ÜYE EKLEME (SAYFAYI AÇ)
        public IActionResult Create()
        {
            return View();
        }

        // 2. YENİ ÜYE EKLEME (KAYDET)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            user.RegistrationDate = DateTime.Now; 
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 3. ÜYE DÜZENLEME (SAYFAYI AÇ)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // 3. ÜYE DÜZENLEME (GÜNCELLE)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId) return NotFound();

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.UserId == id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // 4. SİLME İŞLEMİ
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}