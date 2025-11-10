using Microsoft.AspNetCore.Mvc;

namespace AppMascotas.Controllers
{
    public class MapaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}