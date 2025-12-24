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
            // 1. Seçilen kullanıcıyı veritabanından bul (UserManager olmadan)
            // Not: Senin User tablondaki ID string ise bu çalışır, int ise int.Parse(loan.UserId) yapmalısın.
            var secilenUye = await _context.Users.FindAsync(loan.UserId);

            // 2. Varsayılan Süre (Standart Üye)
            int oduncSuresi = 15;

            // 3. Kullanıcıyı bulduysak Rolünü kontrol et
            // Senin Users tablonda "Role" diye bir sütun olduğunu varsayıyorum.
            if (secilenUye != null)
            {
                // Veritabanındaki "Role" sütunu "Akademisyen" mi?
                if (secilenUye.Role == "Akademisyen") 
                {
                    oduncSuresi = 30; // 🎓 Akademisyenlere 30 gün!
                }
            }

            // 4. Tarihleri Ayarla
            loan.BorrowDate = DateOnly.FromDateTime(DateTime.Now);
            loan.DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(oduncSuresi)); 
            loan.Status = "active";

            // 5. Servisi çağır ve kaydet
            await _loanService.OduncVer(loan);
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _loanService.OduncIptal(id);
            return RedirectToAction(nameof(Index));
        }
    }
}