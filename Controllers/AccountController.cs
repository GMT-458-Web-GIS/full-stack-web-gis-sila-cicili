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
                    new Claim(ClaimTypes.Name, user.Username?? ""),
                    new Claim(ClaimTypes.Role, user.Role?? "") // EN ÖNEMLİ KISIM: Rolü buraya yüklüyoruz
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Giriş yap
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