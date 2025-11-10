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
    public class MascotasController : Controller
    {
        private readonly EscuelaDatabaseContext _context;
        private readonly UserManager<Veterinaria> _userManager;

        public MascotasController(EscuelaDatabaseContext context, UserManager<Veterinaria> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Mascotas
        public async Task<IActionResult> Index(string searchString)
        {
            var veterinariaId = _userManager.GetUserId(User);
            var mascotasQuery = _context.Mascotas
                .Where(m => m.VeterinariaId == veterinariaId)
                .Include(m => m.Dueno)
                .AsQueryable();

            // Aplicar búsqueda si hay un término
            if (!string.IsNullOrEmpty(searchString))
            {
                mascotasQuery = mascotasQuery.Where(m =>
                    m.Nombre.Contains(searchString) ||
                    m.Especie.Contains(searchString) ||
                    (m.Raza != null && m.Raza.Contains(searchString)) ||
                    m.Dueno.NombreCompleto.Contains(searchString)
                );
                
                ViewData["CurrentFilter"] = searchString;
            }

            var mascotas = await mascotasQuery
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return View(mascotas);
        }

        // GET: Mascotas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            var mascota = await _context.Mascotas
                .Include(m => m.Dueno)
                .Include(m => m.Turnos)
                .FirstOrDefaultAsync(m => m.Id == id && m.VeterinariaId == veterinariaId);
            
            if (mascota == null)
            {
                return NotFound();
            }

            return View(mascota);
        }

        // GET: Mascotas/Create
        public async Task<IActionResult> Create()
        {
            var veterinariaId = _userManager.GetUserId(User);
            ViewData["DuenoId"] = new SelectList(
                await _context.Duenos
                    .Where(d => d.VeterinariaId == veterinariaId)
                    .OrderBy(d => d.NombreCompleto)
                    .ToListAsync(),
                "Id",
                "NombreCompleto"
            );
            return View();
        }

        // POST: Mascotas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Especie,Raza,FechaNacimiento,Sexo,Color,Peso,Observaciones,DuenoId")] Mascota mascota)
        {
            var veterinariaId = _userManager.GetUserId(User)!;
            
            // Remover validaciones de propiedades que no están en el formulario
            ModelState.Remove("VeterinariaId");
            ModelState.Remove("Veterinaria");
            ModelState.Remove("Dueno");
            ModelState.Remove("Turnos");

            if (ModelState.IsValid)
            {
                mascota.VeterinariaId = veterinariaId;
                mascota.FechaRegistro = DateTime.Now;
                
                _context.Add(mascota);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Mascota registrada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["DuenoId"] = new SelectList(
                await _context.Duenos
                    .Where(d => d.VeterinariaId == veterinariaId)
                    .OrderBy(d => d.NombreCompleto)
                    .ToListAsync(),
                "Id",
                "NombreCompleto",
                mascota.DuenoId
            );
            return View(mascota);
        }

        // GET: Mascotas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m => m.Id == id && m.VeterinariaId == veterinariaId);
            
            if (mascota == null)
            {
                return NotFound();
            }
            
            ViewData["DuenoId"] = new SelectList(
                await _context.Duenos
                    .Where(d => d.VeterinariaId == veterinariaId)
                    .OrderBy(d => d.NombreCompleto)
                    .ToListAsync(),
                "Id",
                "NombreCompleto",
                mascota.DuenoId
            );
            return View(mascota);
        }

        // POST: Mascotas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Especie,Raza,FechaNacimiento,Sexo,Color,Peso,Observaciones,FechaRegistro,DuenoId,VeterinariaId")] Mascota mascota)
        {
            if (id != mascota.Id)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            if (mascota.VeterinariaId != veterinariaId)
            {
                return Forbid();
            }

            // Remover validaciones de propiedades de navegación
            ModelState.Remove("Veterinaria");
            ModelState.Remove("Dueno");
            ModelState.Remove("Turnos");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mascota);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Mascota actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MascotaExists(mascota.Id))
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
            
            ViewData["DuenoId"] = new SelectList(
                await _context.Duenos
                    .Where(d => d.VeterinariaId == veterinariaId)
                    .OrderBy(d => d.NombreCompleto)
                    .ToListAsync(),
                "Id",
                "NombreCompleto",
                mascota.DuenoId
            );
            return View(mascota);
        }

        // GET: Mascotas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            var mascota = await _context.Mascotas
                .Include(m => m.Dueno)
                .Include(m => m.Turnos)
                .FirstOrDefaultAsync(m => m.Id == id && m.VeterinariaId == veterinariaId);
            
            if (mascota == null)
            {
                return NotFound();
            }

            return View(mascota);
        }

        // POST: Mascotas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veterinariaId = _userManager.GetUserId(User);
            var mascota = await _context.Mascotas
                .Include(m => m.Turnos)
                .FirstOrDefaultAsync(m => m.Id == id && m.VeterinariaId == veterinariaId);

            if (mascota != null)
            {
                // Primero eliminar todos los turnos asociados
                if (mascota.Turnos.Any())
                {
                    _context.Turnos.RemoveRange(mascota.Turnos);
                }

                // Luego eliminar la mascota
                _context.Mascotas.Remove(mascota);
                
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Mascota {mascota.Nombre} eliminada exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MascotaExists(int id)
        {
            return _context.Mascotas.Any(e => e.Id == id);
        }
    }
}
