using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;

namespace LibrarySystem.Services
{
    public class RequestService : IRequestService
    {
        private readonly KütüphaneeContext _context;

        public RequestService(KütüphaneeContext context)
        {
            _context = context;
        }

        public async Task<List<Request>> BekleyenTalepleriGetir()
        {
            return await _context.Requests
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.Status == "pending")
                .OrderBy(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task TalebiOnayla(int requestId)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request == null) return;

            var book = await _context.Books.FindAsync(request.BookId);
            
            // Stok Kontrolü
            if (book != null && book.CurrentStock > 0)
            {
                // 1. Ödünç Kaydı Oluştur
                var loan = new Loan
                {
                    UserId = request.UserId,
                    BookId = request.BookId,
                    BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                    DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                    Status = "active"
                };
                _context.Loans.Add(loan);

                // 2. Stok Düş
                book.CurrentStock -= 1;
                _context.Update(book);

                // 3. Talebi Sil
                _context.Requests.Remove(request);
                
                // Hepsini tek seferde kaydet (Transaction mantığı)
                await _context.SaveChangesAsync();
            }
        }

        public async Task TalebiReddet(int requestId)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request != null)
            {
                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}