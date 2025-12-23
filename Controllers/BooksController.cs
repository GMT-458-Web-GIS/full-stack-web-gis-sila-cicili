using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization; // Kilit Kütüphanesi

namespace LibrarySystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly KütüphaneeContext _context;

        public BooksController(KütüphaneeContext context)
        {
            _context = context;
        }

        // HERKES GÖREBİLİR (Kilit Yok)
        public async Task<IActionResult> Index(string searchString)
        {
            var books = from b in _context.Books select b;
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString) || s.Author.Contains(searchString));
            }
            return View(await books.ToListAsync());
        }

        // SADECE ADMIN GİREBİLİR (Kilit Var)
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            book.CurrentStock = book.TotalStock;
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var book = await _context.Books.FindAsync(id);
            return View(book);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.BookId) return NotFound();
            _context.Update(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "student")]
public async Task<IActionResult> RequestBook(int id)
{
    var username = User.Identity?.Name;
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    
    if (user == null) return RedirectToAction("Login", "Account");

    // Zaten talep etmiş mi?
    var varMi = await _context.Requests
        .AnyAsync(r => r.BookId == id && r.UserId == user.UserId && r.Status == "pending");

    if (varMi)
    {
        TempData["Message"] = "Bu kitabı zaten talep ettiniz, onay bekleniyor.";
        return RedirectToAction(nameof(Index));
    }

    // Talebi Kaydet
    var yeniTalep = new Request
    {
        UserId = user.UserId,
        BookId = id,
        RequestDate = DateTime.Now,
        Status = "pending"
    };

    _context.Requests.Add(yeniTalep);
    await _context.SaveChangesAsync();

    TempData["Success"] = "Kitap talebiniz alındı! Yönetici onaylayınca teslim alabilirsiniz.";
    return RedirectToAction(nameof(Index));
}
    }




    
}