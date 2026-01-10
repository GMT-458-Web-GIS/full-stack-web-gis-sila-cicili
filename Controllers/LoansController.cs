using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Veritabanı işlemleri için gerekli
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly KütüphaneeContext _context;

        public LoansController(ILoanService loanService, KütüphaneeContext context)
        {
            _loanService = loanService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var loans = await _loanService.TumOduncleriGetir();
            return View(loans);
        }

        // GET: Loans/Create (Sayfa ilk açıldığında çalışan kısım)
        public async Task<IActionResult> Create()
        {
            // 👇 GÜNCELLENEN KISIM: 
            // SelectList yerine tüm kullanıcı listesini ViewBag'e atıyoruz.
            // Böylece View tarafında kullanıcının 'Role' bilgisine erişebileceğiz.
            ViewBag.MembersList = await _context.Users.OrderBy(u => u.FirstName).ToListAsync(); 

            // Kitaplar için sadece ID ve İsim yeterli, o yüzden SelectList kullanmaya devam ediyoruz.
            ViewData["BookId"] = new SelectList(await _loanService.DropdownIcinKitaplar(), "BookId", "Title");
            
            return View();
        }

        // POST: Loans/Create (Ödünç Ver butonuna basılınca çalışan kısım)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loan loan)
        {
            // 1. Kitabı bul ve stok kontrolü yap
            var secilenKitap = await _context.Books.FindAsync(loan.BookId);

            if (secilenKitap != null)
            {
                if (secilenKitap.CurrentStock > 0)
                {
                    secilenKitap.CurrentStock -= 1;
                    _context.Books.Update(secilenKitap);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("", "Bu kitabın stoğu tükenmiştir!");
                    // Hata durumunda listeleri tekrar doldurup sayfayı geri gönderiyoruz
                    ViewBag.MembersList = await _context.Users.OrderBy(u => u.FirstName).ToListAsync();
                    ViewData["BookId"] = new SelectList(await _loanService.DropdownIcinKitaplar(), "BookId", "Title");
                    return View(loan);
                }
            }

            // 2. Kullanıcı Rol Kontrolü ve Süre Hesaplama
            var secilenUye = await _context.Users.FindAsync(loan.UserId);
            int oduncSuresi = 15; // Varsayılan süre

            // Veritabanında rol nasıl kayıtlıysa (Büyük/Küçük harf) hepsini kontrol ediyoruz
            if (secilenUye != null && (secilenUye.Role == "Akademisyen" || secilenUye.Role == "Academic" || secilenUye.Role == "admin")) 
            {
                oduncSuresi = 30;
            }

            // 3. Tarih ve Durum Ayarları
            loan.BorrowDate = DateOnly.FromDateTime(DateTime.Now);
            loan.DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(oduncSuresi)); 
            loan.Status = "active";

            // 4. Ödünç işlemini servisle tamamla
            await _loanService.OduncVer(loan);
            
            return RedirectToAction(nameof(Index));
        }
    }
}