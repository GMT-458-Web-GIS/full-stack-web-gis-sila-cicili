using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem için gerekli
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
            // Rol listesini oluşturup ViewBag ile sayfaya gönderiyoruz
            ViewBag.Roles = GetRolesList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kullanıcı adı kontrolü
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılıyor.");
                    ViewBag.Roles = GetRolesList(); // Hata olursa liste kaybolmasın
                    return View(model);
                }

                // 2. Yeni Kullanıcı Oluştur
                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.Username,
                    PasswordHash = model.Password,
                    // Seçilen rolü atıyoruz, boşsa "student" yapıyoruz
                    Role = !string.IsNullOrEmpty(model.Role) ? model.Role : "student"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Kayıt başarılı! Lütfen giriş yapınız.";
                return RedirectToAction("Login");
            }

            // Hata durumunda listeyi tekrar doldur
            ViewBag.Roles = GetRolesList();
            return View(model);
        }

        // Rol listesini oluşturan yardımcı metot (Kod tekrarını önlemek için)
        private List<SelectListItem> GetRolesList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Öğrenci", Value = "member" }, // Veritabanında 'member' veya 'student' ne kullanıyorsan onu yaz
                new SelectListItem { Text = "Akademisyen", Value = "Akademisyen" },
                new SelectListItem { Text = "Yönetici (Admin)", Value = "admin" }
            };
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
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username ?? ""),
                    new Claim(ClaimTypes.Role, user.Role ?? "")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}