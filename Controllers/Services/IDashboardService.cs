namespace LibrarySystem.Services
{
    public interface IDashboardService
    {
        Task<int> ToplamKitapSayisi();
        Task<int> ToplamUyeSayisi();
        Task<int> AktifOduncSayisi();
    }
}