using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // 👇 Bu kütüphane SelectListItem için gerekli
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.TumUyeleriGetir();
            return View(users);
        }

        // 👇 GÜNCELLENEN KISIM BURASI 👇
        public IActionResult Create()
        {
            // Admin panelinde yeni üye eklerken çıkacak liste
            List<SelectListItem> roller = new List<SelectListItem>
            {
                new SelectListItem { Text = "Öğrenci", Value = "student" }, // veya "member" (veritabanında ne kullanıyorsan)
                new SelectListItem { Text = "Akademisyen", Value = "Akademisyen" }, // 🎓 İşte aradığın özellik
                new SelectListItem { Text = "Yönetici (Admin)", Value = "admin" }
            };

            // Listeyi sayfaya (View) taşıyoruz
            ViewBag.Roles = roller;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            // Eğer rol seçilmediyse varsayılan olarak "student" ata
            if (string.IsNullOrEmpty(user.Role))
            {
                user.Role = "student";
            }

            await _userService.UyeEkle(user);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userService.UyeGetirIdIle(id);
            if (user == null) return NotFound();

            // Düzenleme sayfasında da rol değiştirmek istersen aynı listeyi buraya da eklemelisin
            List<SelectListItem> roller = new List<SelectListItem>
            {
                new SelectListItem { Text = "Öğrenci", Value = "student" },
                new SelectListItem { Text = "Akademisyen", Value = "Akademisyen" },
                new SelectListItem { Text = "Yönetici (Admin)", Value = "admin" }
            };
            ViewBag.Roles = roller;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId) return NotFound();
            
            await _userService.UyeGuncelle(user);
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _userService.UyeSil(id);
            return RedirectToAction(nameof(Index));
        }
    }
}