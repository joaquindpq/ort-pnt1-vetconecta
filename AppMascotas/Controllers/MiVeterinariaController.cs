using AppMascotas.Context;
using AppMascotas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppMascotas.Controllers
{
    [Authorize]
    public class MiVeterinariaController : Controller
    {
        private readonly EscuelaDatabaseContext _context;
        private readonly UserManager<Veterinaria> _userManager;

        public MiVeterinariaController(EscuelaDatabaseContext context, UserManager<Veterinaria> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var veterinariaId = _userManager.GetUserId(User);
            var veterinaria = await _userManager.FindByIdAsync(veterinariaId);

            if (veterinaria == null)
            {
                return NotFound();
            }

            var totalDuenos = await _context.Duenos
                .Where(d => d.VeterinariaId == veterinariaId)
                .CountAsync();

            var totalMascotas = await _context.Mascotas
                .Where(m => m.VeterinariaId == veterinariaId)
                .CountAsync();

            var totalTurnos = await _context.Turnos
                .Where(t => t.VeterinariaId == veterinariaId)
                .CountAsync();

            var turnosPendientes = await _context.Turnos
                .Where(t => t.VeterinariaId == veterinariaId && t.Estado == "Pendiente")
                .CountAsync();

            var turnosHoy = await _context.Turnos
                .Where(t => t.VeterinariaId == veterinariaId && 
                           t.FechaHora.Date == DateTime.Today)
                .CountAsync();

            var ultimosDuenos = await _context.Duenos
                .Where(d => d.VeterinariaId == veterinariaId)
                .OrderByDescending(d => d.FechaRegistro)
                .Take(5)
                .Include(d => d.Mascotas)
                .ToListAsync();

            var ultimasMascotas = await _context.Mascotas
                .Where(m => m.VeterinariaId == veterinariaId)
                .OrderByDescending(m => m.FechaRegistro)
                .Take(5)
                .Include(m => m.Dueno)
                .ToListAsync();

            var proximosTurnos = await _context.Turnos
                .Where(t => t.VeterinariaId == veterinariaId && 
                           t.FechaHora >= DateTime.Now)
                .OrderBy(t => t.FechaHora)
                .Take(5)
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Dueno)
                .ToListAsync();

            var distribucionEspecies = await _context.Mascotas
                .Where(m => m.VeterinariaId == veterinariaId)
                .GroupBy(m => m.Especie)
                .Select(g => new { Especie = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .ToListAsync();

            ViewBag.Veterinaria = veterinaria;
            ViewBag.TotalDuenos = totalDuenos;
            ViewBag.TotalMascotas = totalMascotas;
            ViewBag.TotalTurnos = totalTurnos;
            ViewBag.TurnosPendientes = turnosPendientes;
            ViewBag.TurnosHoy = turnosHoy;
            ViewBag.UltimosDuenos = ultimosDuenos;
            ViewBag.UltimasMascotas = ultimasMascotas;
            ViewBag.ProximosTurnos = proximosTurnos;
            ViewBag.DistribucionEspecies = distribucionEspecies;

            return View();
        }

        public async Task<IActionResult> Edit()
        {
            var veterinariaId = _userManager.GetUserId(User);
            var veterinaria = await _userManager.FindByIdAsync(veterinariaId);

            if (veterinaria == null)
            {
                return NotFound();
            }

            return View(veterinaria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,NombreVeterinaria,Email,PhoneNumber,Direccion")] Veterinaria veterinaria)
        {
            var veterinariaId = _userManager.GetUserId(User);
            
            if (veterinaria.Id != veterinariaId)
            {
                return Forbid();
            }

            ModelState.Remove("Duenos");
            ModelState.Remove("Mascotas");
            ModelState.Remove("Turnos");

            if (ModelState.IsValid)
            {
                try
                {
                    var veterinariaActual = await _userManager.FindByIdAsync(veterinariaId);
                    
                    if (veterinariaActual == null)
                    {
                        return NotFound();
                    }

                    veterinariaActual.NombreVeterinaria = veterinaria.NombreVeterinaria;
                    veterinariaActual.PhoneNumber = veterinaria.PhoneNumber;
                    veterinariaActual.Direccion = veterinaria.Direccion;

                    var result = await _userManager.UpdateAsync(veterinariaActual);
                    
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Información de la veterinaria actualizada exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar. Por favor, intenta nuevamente.");
                }
            }
            
            return View(veterinaria);
        }
    }
}
