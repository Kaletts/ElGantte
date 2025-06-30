using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElGantte.Data;
using ElGantte.Models;

namespace ElGantte.Controllers
{
    public class TelecertificacionesController : Controller
    {
        private readonly AppDbContext _context;

        public TelecertificacionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Telecertificaciones
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Telecertificaciones.Include(t => t.Integracion);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Telecertificaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telecertificaciones = await _context.Telecertificaciones
                .Include(t => t.Integracion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telecertificaciones == null)
            {
                return NotFound();
            }

            return View(telecertificaciones);
        }

        // GET: Telecertificaciones/Create
        public IActionResult Create()
        {
            ViewData["IntegracioneId"] = new SelectList(_context.Integraciones, "Id", "Id");
            return View();
        }

        // POST: Telecertificaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Enlace,Fecha,Descripcion,IntegracioneId")] Telecertificaciones telecertificaciones)
        {
            if (ModelState.IsValid)
            {
                _context.Add(telecertificaciones);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Telecertificación cargada";
                return RedirectToAction("Edit", "Integraciones", new { id = telecertificaciones.IntegracioneId });
            } else
            {
                TempData["Error"] = "Error al guardar Telecertificación";
            }
                ViewData["IntegracioneId"] = new SelectList(_context.Integraciones, "Id", "Id", telecertificaciones.IntegracioneId);
            return RedirectToAction("Edit", "Integraciones", new { id = telecertificaciones.IntegracioneId });
        }

        // GET: Telecertificaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telecertificaciones = await _context.Telecertificaciones.FindAsync(id);
            if (telecertificaciones == null)
            {
                return NotFound();
            }
            ViewData["IntegracioneId"] = new SelectList(_context.Integraciones, "Id", "Id", telecertificaciones.IntegracioneId);
            return View(telecertificaciones);
        }

        // POST: Telecertificaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Enlace,Fecha,Descripcion,IntegracioneId")] Telecertificaciones telecertificaciones)
        {
            if (id != telecertificaciones.Id)
            {
                TempData["Error"] = "Telecertificación no encontrada";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telecertificaciones);
                    TempData["Success"] = "Telecertificación guardada correctamente";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelecertificacionesExists(telecertificaciones.Id))
                    {
                        TempData["Error"] = "Error al guardar Telecertificación";
                        return NotFound();
                    }
                    else
                    {
                        TempData["Error"] = "Error al guardar Telecertificación";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IntegracioneId"] = new SelectList(_context.Integraciones, "Id", "Id", telecertificaciones.IntegracioneId);
            return View(telecertificaciones);
        }

        // GET: Telecertificaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Telecertificación no encontrada";
                return NotFound();
            }

            var telecertificaciones = await _context.Telecertificaciones
                .Include(t => t.Integracion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telecertificaciones == null)
            {
                TempData["Error"] = "Telecertificación no encontrada";
                return NotFound();
            }

            return View(telecertificaciones);
        }

        // POST: Telecertificaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var telecertificaciones = await _context.Telecertificaciones.FindAsync(id);
            if (telecertificaciones != null)
            {
                _context.Telecertificaciones.Remove(telecertificaciones);
            }

            await _context.SaveChangesAsync();
            TempData["Error"] = "Telecertificación borrada";
            return RedirectToAction(nameof(Index));
        }

        private bool TelecertificacionesExists(int id)
        {
            return _context.Telecertificaciones.Any(e => e.Id == id);
        }
    }
}
