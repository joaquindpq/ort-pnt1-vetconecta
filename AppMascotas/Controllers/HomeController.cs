using System.Diagnostics;
using AppMascotas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AppMascotas.Context;
using Microsoft.EntityFrameworkCore;

namespace AppMascotas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Veterinaria> _userManager;
        private readonly EscuelaDatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<Veterinaria> userManager, EscuelaDatabaseContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    ViewBag.NombreVeterinaria = user.NombreVeterinaria;
                    ViewBag.IsAuthenticated = true;
                }
                else
                {
                    ViewBag.IsAuthenticated = false;
                }
            }
            else
            {
                ViewBag.IsAuthenticated = false;
            }
            return View();
        }

        [Route("landing-registro-mascotas")]
        public IActionResult LandingRegistroMascotas()
        {
            return View("~/Views/Home/landings/LandingRegistroMascotas.cshtml");
        }

        [Route("landing-gestion-duenos")]
        public IActionResult LandingGestionDuenos()
        {
            return View("~/Views/Home/landings/LandingGestionDuenos.cshtml");
        }

        [Route("landing-turnos-veterinarios")]
        public IActionResult LandingTurnosVeterinarios()
        {
            return View("~/Views/Home/landings/LandingTurnosVeterinarios.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
