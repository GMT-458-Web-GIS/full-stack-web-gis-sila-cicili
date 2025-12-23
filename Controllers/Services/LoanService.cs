using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;

namespace LibrarySystem.Services
{
    public class LoanService : ILoanService
    {
        private readonly KütüphaneeContext _context;

        public LoanService(KütüphaneeContext context)
        {
            _context = context;
        }

        public async Task<List<Loan>> TumOduncleriGetir()
        {
            // Include ile Kitap ve Üye isimlerini de getiriyoruz
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .OrderByDescending(l => l.BorrowDate) // En son alanı en üstte göster
                .ToListAsync();
        }

        public async Task OduncVer(Loan loan)
        {
            // 1. Kitabı bul
            var book = await _context.Books.FindAsync(loan.BookId);
            if (book != null && book.CurrentStock > 0)
            {
                // 2. Stok düş
                book.CurrentStock -= 1;
                _context.Update(book);

                // 3. Ödünç kaydını ekle
                _context.Add(loan);
                await _context.SaveChangesAsync();
            }
        }

        public async Task OduncIptal(int id)
        {
            // İade alma veya silme işlemi
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
            {
                // 1. Kitabı bul ve Stoğu Artır
                var book = await _context.Books.FindAsync(loan.BookId);
                if (book != null)
                {
                    book.CurrentStock += 1;
                    _context.Update(book);
                }

                // 2. Kaydı sil
                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
            }
        }

        // Dropdown doldurmak için yardımcı metodlar
        public async Task<List<User>> DropdownIcinUyeler() => await _context.Users.ToListAsync();
        public async Task<List<Book>> DropdownIcinKitaplar() => await _context.Books.Where(b => b.CurrentStock > 0).ToListAsync();
    }
}