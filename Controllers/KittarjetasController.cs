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
    public class KittarjetasController : Controller
    {
        private readonly AppDbContext _context;

        public KittarjetasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Kittarjetas
        // GET: Index
        public async Task<IActionResult> Index()
        {
            var kits = await _context.KitTarjetas
                .Include(k => k.Integracion)
                    .ThenInclude(i => i.PartnerNavigation)
                .Include(k => k.Tarjetas)
                .ToListAsync();

            return View(kits);
        }

        // POST: Crear Kit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateKit(string Nombre, int? IntegracionId)
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                TempData["Error"] = "El nombre del kit no puede estar vacío.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si ya existe el nombre
            bool exists = await _context.KitTarjetas.AnyAsync(k => k.Nombre == Nombre);
            if (exists)
            {
                TempData["Warning"] = "Ya existe un kit con ese nombre. Usa otro nombre.";
                return RedirectToAction(nameof(Index));
            }

            var kit = new Kittarjeta
            {
                Nombre = Nombre,
                IntegracionId = IntegracionId,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };

            _context.KitTarjetas.Add(kit);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Kit '{Nombre}' creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Crear Tarjeta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTarjeta(string Numero, string Tipo, string PIN, int? KitTarjetaId)
        {
            if (string.IsNullOrWhiteSpace(Numero) || string.IsNullOrWhiteSpace(Tipo) || string.IsNullOrWhiteSpace(PIN))
            {
                TempData["Error"] = "Todos los campos de la tarjeta son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            if (!KitTarjetaId.HasValue)
            {
                TempData["Error"] = "Debes seleccionar un kit para asignar la tarjeta.";
                return RedirectToAction(nameof(Index));
            }

            var tarjeta = new Tarjetas
            {
                Numero = Numero,
                Tipo = Tipo,
                PIN = PIN,
                KitTarjetaId = KitTarjetaId.Value
            };

            _context.Tarjetas.Add(tarjeta);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Tarjeta '{Numero}' creada y asignada al kit correctamente.";
            return RedirectToAction(nameof(Index));
        }


        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // GET: Kittarjetas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kittarjeta = await _context.KitTarjetas
                .Include(k => k.Integracion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kittarjeta == null)
            {
                return NotFound();
            }

            return View(kittarjeta);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // POST: Kittarjetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kittarjeta = await _context.KitTarjetas.FindAsync(id);
            if (kittarjeta != null)
            {
                _context.KitTarjetas.Remove(kittarjeta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
