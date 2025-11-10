using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppMascotas.Context;
using AppMascotas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AppMascotas.Controllers
{
    [Authorize]
    public class TurnosController : Controller
    {
        private readonly EscuelaDatabaseContext _context;
        private readonly UserManager<Veterinaria> _userManager;

        public TurnosController(EscuelaDatabaseContext context, UserManager<Veterinaria> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string estado)
        {
            var veterinariaId = _userManager.GetUserId(User);
            var turnosQuery = _context.Turnos
                .Where(t => t.VeterinariaId == veterinariaId)
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Dueno)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                turnosQuery = turnosQuery.Where(t => t.Estado == estado);
                ViewData["EstadoActual"] = estado;
            }

            var turnos = await turnosQuery
                .OrderByDescending(t => t.FechaHora)
                .ToListAsync();

            return View(turnos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            var turno = await _context.Turnos
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Dueno)
                .FirstOrDefaultAsync(m => m.Id == id && m.VeterinariaId == veterinariaId);
            
            if (turno == null)
            {
                return NotFound();
            }

            return View(turno);
        }

        public async Task<IActionResult> Create()
        {
            var veterinariaId = _userManager.GetUserId(User);
            ViewData["MascotaId"] = new SelectList(
                await _context.Mascotas
                    .Where(m => m.VeterinariaId == veterinariaId)
                    .Include(m => m.Dueno)
                    .Select(m => new { m.Id, NombreMascota = m.Nombre + " - " + m.Dueno.NombreCompleto })
                    .OrderBy(m => m.NombreMascota)
                    .ToListAsync(),
                "Id",
                "NombreMascota"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaHora,Motivo,Observaciones,MascotaId")] Turno turno)
        {
            var veterinariaId = _userManager.GetUserId(User)!;
            
            ModelState.Remove("VeterinariaId");
            ModelState.Remove("Veterinaria");
            ModelState.Remove("Mascota");
            ModelState.Remove("Estado");

            if (ModelState.IsValid)
            {
                turno.VeterinariaId = veterinariaId;
                turno.Estado = "Pendiente";
                turno.FechaCreacion = DateTime.Now;
                
                _context.Add(turno);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Turno registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["MascotaId"] = new SelectList(
                await _context.Mascotas
                    .Where(m => m.VeterinariaId == veterinariaId)
                    .Include(m => m.Dueno)
                    .Select(m => new { m.Id, NombreMascota = m.Nombre + " - " + m.Dueno.NombreCompleto })
                    .OrderBy(m => m.NombreMascota)
                    .ToListAsync(),
                "Id",
                "NombreMascota",
                turno.MascotaId
            );
            return View(turno);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            var turno = await _context.Turnos
                .FirstOrDefaultAsync(t => t.Id == id && t.VeterinariaId == veterinariaId);
            
            if (turno == null)
            {
                return NotFound();
            }
            
            ViewData["MascotaId"] = new SelectList(
                await _context.Mascotas
                    .Where(m => m.VeterinariaId == veterinariaId)
                    .Include(m => m.Dueno)
                    .Select(m => new { m.Id, NombreMascota = m.Nombre + " - " + m.Dueno.NombreCompleto })
                    .OrderBy(m => m.NombreMascota)
                    .ToListAsync(),
                "Id",
                "NombreMascota",
                turno.MascotaId
            );
            return View(turno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaHora,Motivo,Observaciones,Estado,FechaCreacion,MascotaId,VeterinariaId")] Turno turno)
        {
            if (id != turno.Id)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            if (turno.VeterinariaId != veterinariaId)
            {
                return Forbid();
            }

            ModelState.Remove("Veterinaria");
            ModelState.Remove("Mascota");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(turno);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Turno actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurnoExists(turno.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["MascotaId"] = new SelectList(
                await _context.Mascotas
                    .Where(m => m.VeterinariaId == veterinariaId)
                    .Include(m => m.Dueno)
                    .Select(m => new { m.Id, NombreMascota = m.Nombre + " - " + m.Dueno.NombreCompleto })
                    .OrderBy(m => m.NombreMascota)
                    .ToListAsync(),
                "Id",
                "NombreMascota",
                turno.MascotaId
            );
            return View(turno);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            var turno = await _context.Turnos
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Dueno)
                .FirstOrDefaultAsync(m => m.Id == id && m.VeterinariaId == veterinariaId);
            
            if (turno == null)
            {
                return NotFound();
            }

            return View(turno);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veterinariaId = _userManager.GetUserId(User);
            var turno = await _context.Turnos
                .FirstOrDefaultAsync(t => t.Id == id && t.VeterinariaId == veterinariaId);

            if (turno != null)
            {
                _context.Turnos.Remove(turno);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Turno eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TurnoExists(int id)
        {
            return _context.Turnos.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetTurnosJson()
        {
            var veterinariaId = _userManager.GetUserId(User);
            var turnos = await _context.Turnos
                .Where(t => t.VeterinariaId == veterinariaId)
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Dueno)
                .OrderBy(t => t.FechaHora)
                .ToListAsync();

            var eventos = turnos.Select(t => new
            {
                id = t.Id,
                title = $"{t.Mascota?.Nombre} - {t.Motivo}",
                start = t.FechaHora.ToString("yyyy-MM-ddTHH:mm:ss"),
                backgroundColor = t.Estado switch
                {
                    "Pendiente" => "#ffc107",
                    "Realizado" => "#28a745",
                    "Cancelado" => "#dc3545",
                    _ => "#6c757d"
                },
                borderColor = t.Estado switch
                {
                    "Pendiente" => "#ffc107",
                    "Realizado" => "#28a745",
                    "Cancelado" => "#dc3545",
                    _ => "#6c757d"
                },
                textColor = t.Estado == "Pendiente" ? "#000" : "#fff",
                extendedProps = new
                {
                    mascota = t.Mascota?.Nombre,
                    dueno = t.Mascota?.Dueno?.NombreCompleto,
                    motivo = t.Motivo,
                    estado = t.Estado,
                    observaciones = t.Observaciones
                }
            });

            return Json(eventos);
        }
    }
}
