using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;

namespace LibrarySystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly KütüphaneeContext _context;

        public DashboardService(KütüphaneeContext context)
        {
            _context = context;
        }

        public async Task<int> ToplamKitapSayisi()
        {
            return await _context.Books.CountAsync();
        }

        public async Task<int> ToplamUyeSayisi()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> AktifOduncSayisi()
        {
            // Sadece aktif olanları veya tümünü sayabilirsin
            return await _context.Loans.CountAsync(); 
        }
    }
}