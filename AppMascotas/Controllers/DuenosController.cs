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
    public class DuenosController : Controller
    {
        private readonly EscuelaDatabaseContext _context;
        private readonly UserManager<Veterinaria> _userManager;

        public DuenosController(EscuelaDatabaseContext context, UserManager<Veterinaria> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var veterinariaId = _userManager.GetUserId(User);
            
            var duenos = _context.Duenos
                .Where(d => d.VeterinariaId == veterinariaId)
                .Include(d => d.Veterinaria)
                .Include(d => d.Mascotas)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                duenos = duenos.Where(d => d.NombreCompleto.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            return View(await duenos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            var dueno = await _context.Duenos
                .Include(d => d.Veterinaria)
                .Include(d => d.Mascotas)
                .FirstOrDefaultAsync(m => m.Id == id && m.VeterinariaId == veterinariaId);
            
            if (dueno == null)
            {
                return NotFound();
            }

            return View(dueno);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreCompleto,Dni,Telefono,Email,Direccion")] Dueno dueno)
        {
            var veterinariaId = _userManager.GetUserId(User)!;
            
            ModelState.Remove("VeterinariaId");
            ModelState.Remove("Veterinaria");
            ModelState.Remove("Mascotas");
            
            var dniExistente = await _context.Duenos
                .AnyAsync(d => d.Dni == dueno.Dni && d.VeterinariaId == veterinariaId);
            
            if (dniExistente)
            {
                ModelState.AddModelError("Dni", "Ya existe un dueño registrado con este DNI en tu veterinaria.");
            }

            if (ModelState.IsValid)
            {
                dueno.VeterinariaId = veterinariaId;
                dueno.FechaRegistro = DateTime.Now;
                
                _context.Add(dueno);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Dueño registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(dueno);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            var dueno = await _context.Duenos
                .FirstOrDefaultAsync(d => d.Id == id && d.VeterinariaId == veterinariaId);
            
            if (dueno == null)
            {
                return NotFound();
            }
            
            return View(dueno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreCompleto,Dni,Telefono,Email,Direccion,FechaRegistro,VeterinariaId")] Dueno dueno)
        {
            if (id != dueno.Id)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            if (dueno.VeterinariaId != veterinariaId)
            {
                return Forbid();
            }

            ModelState.Remove("Veterinaria");
            ModelState.Remove("Mascotas");

            var dniExistente = await _context.Duenos
                .AnyAsync(d => d.Dni == dueno.Dni && d.VeterinariaId == veterinariaId && d.Id != id);
            
            if (dniExistente)
            {
                ModelState.AddModelError("Dni", "Ya existe otro dueño registrado con este DNI en tu veterinaria.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dueno);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Dueño actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DuenoExists(dueno.Id))
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
            
            return View(dueno);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinariaId = _userManager.GetUserId(User);
            var dueno = await _context.Duenos
                .Include(d => d.Veterinaria)
                .Include(d => d.Mascotas)
                .FirstOrDefaultAsync(m => m.Id == id && m.VeterinariaId == veterinariaId);
            
            if (dueno == null)
            {
                return NotFound();
            }

            return View(dueno);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veterinariaId = _userManager.GetUserId(User);
            var dueno = await _context.Duenos
                .Include(d => d.Mascotas)
                    .ThenInclude(m => m.Turnos)
                .FirstOrDefaultAsync(d => d.Id == id && d.VeterinariaId == veterinariaId);

            if (dueno != null)
            {
                foreach (var mascota in dueno.Mascotas.ToList())
                {
                    if (mascota.Turnos.Any())
                    {
                        _context.Turnos.RemoveRange(mascota.Turnos);
                    }
                }

                if (dueno.Mascotas.Any())
                {
                    _context.Mascotas.RemoveRange(dueno.Mascotas);
                }

                _context.Duenos.Remove(dueno);
                
                await _context.SaveChangesAsync();
                
                var cantidadMascotas = dueno.Mascotas.Count;
                TempData["SuccessMessage"] = $"Dueño eliminado exitosamente junto con {cantidadMascotas} mascota(s) asociada(s).";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DuenoExists(int id)
        {
            return _context.Duenos.Any(e => e.Id == id);
        }
    }
}
