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
    public class CartascesionController : Controller
    {
        private readonly AppDbContext _context;

        public CartascesionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Cartascesion
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cartascesions.ToListAsync());
        }

        // GET: Cartascesion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartascesion = await _context.Cartascesions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartascesion == null)
            {
                return NotFound();
            }

            return View(cartascesion);
        }

        // GET: Cartascesion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cartascesion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirCarta(int integracionId, IFormFile ArchivoCartaCesion)
        {
            if (ArchivoCartaCesion == null || ArchivoCartaCesion.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo válido.";
                return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
            }

            if (ArchivoCartaCesion.Length > 50 * 1024 * 1024)
            {
                TempData["Error"] = "El archivo excede los 50MB permitidos.";
                return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
            }

            using var memoryStream = new MemoryStream();
            await ArchivoCartaCesion.CopyToAsync(memoryStream);

            var carta = new Cartascesion
            {
                CartasCesion1 = memoryStream.ToArray(),
                NombreArchivo = Path.GetFileName(ArchivoCartaCesion.FileName),
                TipoMime = ArchivoCartaCesion.ContentType,
                Fecha = DateOnly.FromDateTime(DateTime.Today)
            };

            _context.Cartascesions.Add(carta);
            await _context.SaveChangesAsync();

            // Asociar la carta a la integración
            var integracion = await _context.Integraciones.FindAsync(integracionId);
            if (integracion != null)
            {
                integracion.CartaCesion = carta.Id;
                _context.Update(integracion);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Carta de cesión subida correctamente.";
            return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
        }

        //Metodo para descargar la carta de cesión
        public async Task<IActionResult> Descargar(int id)
        {
            var carta = await _context.Cartascesions.FindAsync(id);
            if (carta == null || carta.CartasCesion1 == null)
            {
                return NotFound();
            }

            var nombre = carta.NombreArchivo ?? $"Carta_{id}.pdf";
            var tipo = carta.TipoMime ?? "application/octet-stream";

            return File(carta.CartasCesion1, tipo, nombre);
        }


        // GET: Cartascesion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartascesion = await _context.Cartascesions.FindAsync(id);
            if (cartascesion == null)
            {
                return NotFound();
            }
            return View(cartascesion);
        }

        // POST: Cartascesion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CartasCesion1,Fecha")] Cartascesion cartascesion)
        {
            if (id != cartascesion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartascesion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartascesionExists(cartascesion.Id))
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
            return View(cartascesion);
        }

        // GET: Cartascesion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartascesion = await _context.Cartascesions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartascesion == null)
            {
                return NotFound();
            }

            return View(cartascesion);
        }

        // POST: Cartascesion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartascesion = await _context.Cartascesions.FindAsync(id);
            if (cartascesion != null)
            {
                _context.Cartascesions.Remove(cartascesion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartascesionExists(int id)
        {
            return _context.Cartascesions.Any(e => e.Id == id);
        }
    }
}
