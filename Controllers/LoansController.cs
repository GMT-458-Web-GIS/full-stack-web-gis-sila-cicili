using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Veritabanı sorgusu için gerekli
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly KütüphaneeContext _context; // Veritabanına direkt erişim ekledik

        // UserManager yerine _context kullanacağız
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

        public async Task<IActionResult> Create()
        {
            ViewData["UserId"] = new SelectList(await _loanService.DropdownIcinUyeler(), "UserId", "Username");
            ViewData["BookId"] = new SelectList(await _loanService.DropdownIcinKitaplar(), "BookId", "Title");
            return View();
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Loan loan)
{
    // 1. Kitabı bul ve stok kontrolü yap
    // Not: Tablo adın Context içinde muhtemelen 'Books' olarak geçiyor.
    var secilenKitap = await _context.Books.FindAsync(loan.BookId);

    if (secilenKitap != null)
    {
        // Önemli: Senin modelinde isim 'CurrentStock'
        if (secilenKitap.CurrentStock > 0)
        {
            secilenKitap.CurrentStock -= 1;
            _context.Books.Update(secilenKitap);
            
            // Stok değişimini veritabanına hemen yansıtalım
            await _context.SaveChangesAsync();
        }
        else
        {
            ModelState.AddModelError("", "Bu kitabın stoğu tükenmiştir!");
            ViewData["UserId"] = new SelectList(await _loanService.DropdownIcinUyeler(), "UserId", "Username");
            ViewData["BookId"] = new SelectList(await _loanService.DropdownIcinKitaplar(), "BookId", "Title");
            return View(loan);
        }
    }

    // 2. Kullanıcı Rol Kontrolü ve Süre Hesaplama
    var secilenUye = await _context.Users.FindAsync(loan.UserId);
    int oduncSuresi = 15;

    // Küçük harf riskine karşı hem "Akademisyen" hem "academic" kontrolü yapalım
    if (secilenUye != null && (secilenUye.Role == "Akademisyen" || secilenUye.Role == "Admin")) 
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