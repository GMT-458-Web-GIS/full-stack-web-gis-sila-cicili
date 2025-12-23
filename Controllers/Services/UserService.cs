using Microsoft.EntityFrameworkCore;
using LibrarySystem.Models;

namespace LibrarySystem.Services
{
    public class UserService : IUserService
    {
        private readonly KütüphaneeContext _context;

        public UserService(KütüphaneeContext context)
        {
            _context = context;
        }

        public async Task<List<User>> TumUyeleriGetir()
        {
            return await _context.Users.OrderBy(u => u.UserId).ToListAsync();
        }

        public async Task<User?> UyeGetirIdIle(int? id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task UyeEkle(User user)
        {
            // Kayıt tarihi otomatik olsun
            user.RegistrationDate = DateTime.Now; 
            _context.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UyeGuncelle(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UyeSil(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}