using ElGantte.Data;
using ElGantte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGantte.Controllers
{
    [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
    public class TarjetasController : Controller
    {
        private readonly AppDbContext _context;

        public TarjetasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Tarjetas
        public async Task<IActionResult> Index()
        {
            var kits = await _context.KitTarjetas
                 .Include(k => k.Tarjetas)
                 .Include(k => k.Integracion)
                     .ThenInclude(i => i.PartnerNavigation)
                 .ToListAsync();

            return View(kits);
        }

        // POST: Crear Kit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateKit(string nombre, int? integracionId)
        {
            var kit = new Kittarjeta
            {
                Nombre = nombre,
                IntegracionId = integracionId
            };
            _context.KitTarjetas.Add(kit);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kit creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Crear Tarjeta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTarjeta(string numero, string tipo, string pin, int kitTarjetaId)
        {
            if (kitTarjetaId == 0)
            {
                TempData["Error"] = "La tarjeta debe ir asociada a un Kit.";
                return RedirectToAction(nameof(Index));
            }

            var tarjeta = new Tarjetas
            {
                Numero = numero,
                Tipo = tipo,
                PIN = pin,
                KitTarjetaId = kitTarjetaId
            };
            _context.Tarjetas.Add(tarjeta);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Tarjeta creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Eliminar Kit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteKit(int id)
        {
            var kit = await _context.KitTarjetas
                .Include(k => k.Tarjetas) // Incluimos solo Tarjetas
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kit != null)
            {
                _context.Tarjetas.RemoveRange(kit.Tarjetas);
                _context.KitTarjetas.Remove(kit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kit eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Eliminar Tarjeta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTarjeta(int id)
        {
            var tarjeta = await _context.Tarjetas.FindAsync(id);
            if (tarjeta != null)
            {
                _context.Tarjetas.Remove(tarjeta);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tarjeta eliminada correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Editar Tarjeta Inline
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTarjeta(int id, string tipo, string numero, string pin)
        {
            var tarjeta = await _context.Tarjetas.FindAsync(id);
            if (tarjeta != null)
            {
                tarjeta.Tipo = tipo;
                tarjeta.Numero = numero;
                tarjeta.PIN = pin;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tarjeta actualizada correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Tarjetas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarjetas = await _context.Tarjetas
                .Include(t => t.KitTarjeta)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarjetas == null)
            {
                return NotFound();
            }

            return View(tarjetas);
        }

        // GET: Tarjetas/Create
        public IActionResult Create()
        {
            ViewData["KitTarjetaId"] = new SelectList(_context.KitTarjetas, "Id", "Id");
            return View();
        }

        // POST: Tarjetas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo,Numero,PIN,KitTarjetaId")] Tarjetas tarjetas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarjetas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KitTarjetaId"] = new SelectList(_context.KitTarjetas, "Id", "Id", tarjetas.KitTarjetaId);
            return View(tarjetas);
        }

        // GET: Tarjetas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarjetas = await _context.Tarjetas.FindAsync(id);
            if (tarjetas == null)
            {
                return NotFound();
            }
            ViewData["KitTarjetaId"] = new SelectList(_context.KitTarjetas, "Id", "Id", tarjetas.KitTarjetaId);
            return View(tarjetas);
        }

        // POST: Tarjetas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Numero,PIN,KitTarjetaId")] Tarjetas tarjetas)
        {
            if (id != tarjetas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarjetas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarjetasExists(tarjetas.Id))
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
            ViewData["KitTarjetaId"] = new SelectList(_context.KitTarjetas, "Id", "Id", tarjetas.KitTarjetaId);
            return View(tarjetas);
        }

        // GET: Tarjetas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarjetas = await _context.Tarjetas
                .Include(t => t.KitTarjeta)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarjetas == null)
            {
                return NotFound();
            }

            return View(tarjetas);
        }

        // POST: Tarjetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarjetas = await _context.Tarjetas.FindAsync(id);
            if (tarjetas != null)
            {
                _context.Tarjetas.Remove(tarjetas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarjetasExists(int id)
        {
            return _context.Tarjetas.Any(e => e.Id == id);
        }
    }
}
