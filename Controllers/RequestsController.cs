using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Services;
using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly KütüphaneeContext _context;

        public RequestsController(IRequestService requestService, KütüphaneeContext context)
        {
            _requestService = requestService;
            _context = context;
        }

        // --- KULLANICI İŞLEMLERİ ---

        public async Task<IActionResult> Create(int bookId)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");

            await _requestService.TalepOlustur(username, bookId);
            TempData["Message"] = "Talebiniz alındı.";
            return RedirectToAction("Index", "Books");
        }

        public async Task<IActionResult> MyRequests()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");
            var myRequests = await _requestService.KullaniciTalepleriniGetir(username);
            return View(myRequests);
        }

        // --- YÖNETİCİ İŞLEMLERİ ---

        [Authorize(Roles = "Admin, admin")] 
        public async Task<IActionResult> Index()
        {
            // Bekleyenler hariç diğerlerini gizle
            var requests = await _context.Requests
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.Status != "Approved" && r.Status != "Rejected") 
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        [Authorize(Roles = "Admin, admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.Requests
                                        .Include(r => r.User) 
                                        .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null) return RedirectToAction(nameof(Index));
            if (!request.BookId.HasValue) return RedirectToAction(nameof(Index));

            var book = await _context.Books.FindAsync(request.BookId.Value);

            if (book == null)
            {
                TempData["Error"] = "Kitap bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            // STOK KONTROLÜ
            if (book.CurrentStock > 0)
            {
                // 1. Stok Düşür
                book.CurrentStock -= 1;
                _context.Books.Update(book);

                // 2. Talebi Onayla
                request.Status = "Approved";
                _context.Requests.Update(request);

                // 3. Ödünç Kaydı (Loan) Oluştur
                int oduncSuresi = 15; 
                if (request.User != null && (request.User.Role == "Akademisyen" || request.User.Role == "academic"))
                {
                    oduncSuresi = 30;
                }

                var newLoan = new Loan
                {
                    BookId = request.BookId,
                    UserId = request.UserId,
                    BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                    DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(oduncSuresi)),
                    Status = "active"
                };
                _context.Loans.Add(newLoan);

                await _context.SaveChangesAsync();
                
                // Başarılı Mesajı
                TempData["Message"] = "Talep ONAYLANDI, stok düştü ve ödünç verildi.";
            }
            else
            {
                // Stok Yetersiz Mesajı
                TempData["Error"] = "Stok yetersizliğinden reddedildi.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            
            if (request != null)
            {
                request.Status = "Rejected";
                _context.Requests.Update(request);
                await _context.SaveChangesAsync();
                
                // Mesajı düzelttik: Artık "Giriş Reddedildi" gibi anlaşılmayacak
                TempData["Message"] = "Talep REDDEDİLDİ ve listeden kaldırıldı.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}