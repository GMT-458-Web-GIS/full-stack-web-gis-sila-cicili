using LibrarySystem.Models;

namespace LibrarySystem.Services
{
    public interface IRequestService
    {
        Task<List<Request>> BekleyenTalepleriGetir();
        Task TalebiOnayla(int requestId); // En zor işi bu yapacak
        Task TalebiReddet(int requestId);
    }
}