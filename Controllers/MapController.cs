using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    public class MapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
