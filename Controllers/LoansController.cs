using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList için gerekli
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<IActionResult> Index()
        {
            var loans = await _loanService.TumOduncleriGetir();
            return View(loans);
        }

        public async Task<IActionResult> Create()
        {
            // Dropdownları doldur
            ViewData["UserId"] = new SelectList(await _loanService.DropdownIcinUyeler(), "UserId", "Username");
            ViewData["BookId"] = new SelectList(await _loanService.DropdownIcinKitaplar(), "BookId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loan loan)
        {
            // Tarihleri ayarla
            loan.BorrowDate = DateOnly.FromDateTime(DateTime.Now);
            loan.DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(15));
            loan.Status = "active";

            await _loanService.OduncVer(loan); // Servis hem kaydeder hem stok düşer
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _loanService.OduncIptal(id); // Servis hem siler hem stok artırır
            return RedirectToAction(nameof(Index));
        }
    }
}