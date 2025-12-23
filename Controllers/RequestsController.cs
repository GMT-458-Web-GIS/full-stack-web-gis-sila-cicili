using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "admin")] // Sadece Admin
    public class RequestsController : Controller
    {
        private readonly KütüphaneeContext _context;

        public RequestsController(KütüphaneeContext context)
        {
            _context = context;
        }

        // 1. BEKLEYEN TALEPLERİ LİSTELE
        public async Task<IActionResult> Index()
        {
            var requests = await _context.Requests
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.Status == "pending")
                .ToListAsync();
            return View(requests);
        }

        // 2. ONAYLA (Ödünç Ver)
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) return NotFound();

            var book = await _context.Books.FindAsync(request.BookId);
            // Stok kontrolü
            if (book == null || book.CurrentStock <= 0) 
            {
                TempData["Error"] = "Stok yetersiz!";
                return RedirectToAction(nameof(Index));
            }

            // Ödünç Kaydı Oluştur (DateOnly kullandığımız için dönüşüm yaptık)
            var loan = new Loan
            {
                UserId = request.UserId,
                BookId = request.BookId,
                BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                Status = "active"
            };
            _context.Loans.Add(loan);

            // Stok Düş
            book.CurrentStock -= 1;
            _context.Update(book);

            // Talebi Sil (İşlem tamamlandı)
            _context.Requests.Remove(request);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 3. REDDET
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}