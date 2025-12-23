using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization; // <--- KİLİT KÜTÜPHANESİ

namespace LibrarySystem.Controllers
{
    public class LoansController : Controller
    {
        private readonly KütüphaneeContext _context;

        public LoansController(KütüphaneeContext context)
        {
            _context = context;
        }

        // 1. LİSTEYİ HERKES GÖREBİLİR Mİ?
        // İstersen burayı da kilitleyebilirsin. Şimdilik Admin görsün diyelim.
        [Authorize(Roles = "admin")] 
        public async Task<IActionResult> Index()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToListAsync();
            return View(loans);
        }

        // 2. KİTAP VERME SAYFASI (SADECE ADMIN)
        [Authorize(Roles = "admin")] // <--- KİLİT
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username");
            
            var activeBooks = _context.Books.Where(b => b.CurrentStock > 0);
            ViewData["BookId"] = new SelectList(activeBooks, "BookId", "Title");
            
            return View();
        }

        // 2. KİTAP VERME İŞLEMİ (SADECE ADMIN)
        [Authorize(Roles = "admin")] // <--- KİLİT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loan loan)
        {
            loan.BorrowDate = DateOnly.FromDateTime(DateTime.Now);
            loan.DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(15)); 
            loan.Status = "active";

            var book = await _context.Books.FindAsync(loan.BookId);
            if (book != null)
            {
                book.CurrentStock -= 1; 
                _context.Update(book);  
            }

            _context.Add(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 3. İADE ALMA (SADECE ADMIN)
        [Authorize(Roles = "admin")] // <--- KİLİT
        public async Task<IActionResult> Delete(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
            {
                var book = await _context.Books.FindAsync(loan.BookId);
                if (book != null)
                {
                    book.CurrentStock += 1;
                    _context.Update(book);
                }

                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}