using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;

namespace LibrarySystem.Services
{
    // IBookService menüsündeki işleri "uygulayan" (implement) sınıf
    public class BookService : IBookService
    {
        private readonly KütüphaneeContext _context;

        // Dependency Injection ARTIK BURADA YAPILIYOR
        public BookService(KütüphaneeContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> TumKitaplariGetir(string aramaKelimesi)
        {
            var books = from b in _context.Books select b;
            if (!string.IsNullOrEmpty(aramaKelimesi))
            {
                books = books.Where(s => s.Title.Contains(aramaKelimesi) || s.Author.Contains(aramaKelimesi));
            }
            return await books.ToListAsync();
        }

        public async Task<Book?> KitapGetirIdIle(int? id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task YeniKitapEkle(Book book)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task KitapGuncelle(Book book)
        {
            _context.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task KitapSil(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}



