using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class RequestsController : Controller
    {
        private readonly IRequestService _requestService;

        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _requestService.BekleyenTalepleriGetir();
            return View(requests);
        }

        public async Task<IActionResult> Approve(int id)
        {
            // Tüm karmaşık onaylama işlemi serviste yapılıyor
            await _requestService.TalebiOnayla(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int id)
        {
            await _requestService.TalebiReddet(id);
            return RedirectToAction(nameof(Index));
        }
    }
}