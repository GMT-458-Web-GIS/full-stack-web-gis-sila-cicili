using LibrarySystem.Models;

namespace LibrarySystem.Services
{
    public interface IUserService
    {
        Task<List<User>> TumUyeleriGetir();
        Task<User?> UyeGetirIdIle(int? id);
        Task UyeEkle(User user);
        Task UyeGuncelle(User user);
        Task UyeSil(int id);
    }
}