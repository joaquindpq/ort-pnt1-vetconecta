using Microsoft.AspNetCore.Mvc;

namespace AppMascotas.Controllers
{
    public class ContactController : Controller
    {
        [Route("contacto")]
        public IActionResult Index()
        {
            return View();
        }
    }
}