using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Services; // Unutma!

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        // ARTIK CONTEXT YOK, SERVICE VAR
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            // Veritabanı kodları gitti, yerine servis geldi
            var users = await _userService.TumUyeleriGetir();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            await _userService.UyeEkle(user);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userService.UyeGetirIdIle(id); // Servisten çek
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId) return NotFound();
            
            // Burada try-catch bloğu servisin içinde de yönetilebilir 
            // ama şimdilik basit tutalım.
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