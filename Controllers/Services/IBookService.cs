using LibrarySystem.Models; // Modelleri tanıması lazım

namespace LibrarySystem.Services // 👈 İŞTE BURASI BookService İLE AYNI OLMALI
{
    public interface IBookService
    {
        // Tüm Kitapları Getir
        Task<List<Book>> TumKitaplariGetir(string aramaKelimesi);

        // Tek Bir Kitap Getir
        Task<Book?> KitapGetirIdIle(int? id);

        // Yeni Kitap Ekle
        Task YeniKitapEkle(Book book);

        // Kitap Güncelle
        Task KitapGuncelle(Book book);

        // Kitap Sil
        Task KitapSil(int id);
    }
}