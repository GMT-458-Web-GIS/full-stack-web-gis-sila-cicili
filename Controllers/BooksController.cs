using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Services; // Interface'i kullanmak için

namespace LibrarySystem.Controllers
{
    public class BooksController : Controller
    {
        // ARTIK CONTEXT DEĞİL, INTERFACE VAR
        private readonly IBookService _bookService;

        // Constructor'da Interface istiyoruz
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // 1. LİSTELEME
        public async Task<IActionResult> Index(string searchString)
        {
            // İş mantığı Service'e taşındı, biz sadece sonucu alıyoruz
            var books = await _bookService.TumKitaplariGetir(searchString);
            return View(books);
        }

        // 2. EKLEME
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
            book.DateAdded = DateTime.UtcNow; 
            
            // Servisi kullanıyoruz
            await _bookService.YeniKitapEkle(book);
            
            return RedirectToAction(nameof(Index));
        }

        // ... Diğer metodlar da aynı mantıkla değişecek (Edit, Delete vb.)
        // Örneğin Delete:
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookService.KitapSil(id);
            return RedirectToAction(nameof(Index));
        }
        
        // Edit işlemleri için de _bookService.KitapGetirIdIle ve KitapGuncelle kullanmalısın.
    }
}