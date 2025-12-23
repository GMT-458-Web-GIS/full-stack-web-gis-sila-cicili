using LibrarySystem.Models;

namespace LibrarySystem.Services
{
    public interface ILoanService
    {
        Task<List<Loan>> TumOduncleriGetir();
        Task OduncVer(Loan loan); // İçinde stok düşülecek
        Task OduncIptal(int id);  // İçinde stok geri artırılacak (İade mantığı)
        
        // Dropdownlar için veriye ihtiyacımız var
        Task<List<User>> DropdownIcinUyeler();
        Task<List<Book>> DropdownIcinKitaplar();
    }
}