using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly KütüphaneeContext _context;

        public AccountController(KütüphaneeContext context)
        {
            _context = context;
        }

        // ==========================================
        // 👇 KAYIT OLMA (REGISTER) İŞLEMLERİ 👇
        // ==========================================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Bu kullanıcı adı zaten var mı?
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılıyor.");
                    return View(model);
                }

                // 2. Yeni Kullanıcı Oluştur
                // ⚠️ BURASI ÖNEMLİ: Hata almamak için tüm zorunlu alanları dolduruyoruz.
                var newUser = new User
                {
                    FirstName = model.FirstName, // Veritabanındaki 'first_name' hatasını çözer
                    LastName = model.LastName,   // Veritabanındaki 'last_name' için
                    Email = model.Email,         // Veritabanındaki 'email' için
                    Username = model.Username,
                    PasswordHash = model.Password, // Şifreyi veritabanındaki ismine göre atıyoruz
                    Role = "student" // Varsayılan olarak öğrenci
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // 3. Başarılı ise Giriş sayfasına yönlendir
                TempData["Message"] = "Kayıt başarılı! Lütfen giriş yapınız.";
                return RedirectToAction("Login");
            }

            // Hata varsa formu tekrar göster
            return View(model);
        }

        // ==========================================
        // 👇 GİRİŞ YAPMA (LOGIN) İŞLEMLERİ 👇
        // ==========================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Veritabanında bu kullanıcı var mı?
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            if (user != null)
            {
                // Kullanıcı bulundu, kimlik kartını (Cookie) hazırlayalım
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username ?? ""),
                    new Claim(ClaimTypes.Role, user.Role ?? "") // Rolü sisteme tanıtıyoruz
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Giriş yap
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Başarılı giriş sonrası yönlendirme
                return RedirectToAction("Index", "Home"); // Veya "Books"
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        // ==========================================
        // 👇 ÇIKIŞ YAPMA (LOGOUT) İŞLEMLERİ 👇
        // ==========================================

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}