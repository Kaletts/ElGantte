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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirCartaCesion(IFormFile ArchivoCartaCesion, int integracionId)
        {
            if (ArchivoCartaCesion == null || ArchivoCartaCesion.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo válido.";
                return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
            }

            if (ArchivoCartaCesion.Length > 50 * 1024 * 1024)
            {
                TempData["Error"] = "El archivo supera el tamaño máximo permitido de 50 MB.";
                return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
            }

            var nombreArchivo = Path.GetFileName(ArchivoCartaCesion.FileName);
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "CartasCesion", integracionId.ToString());
            Directory.CreateDirectory(folderPath);

            var rutaCompleta = Path.Combine(folderPath, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await ArchivoCartaCesion.CopyToAsync(stream);
            }

            var carta = new Cartascesion
            {
                RutaArchivo = rutaCompleta,
                NombreArchivo = nombreArchivo,
                TipoMime = ArchivoCartaCesion.ContentType,
                Fecha = DateTime.Today,
                IntegracioneId = integracionId
            };

            _context.Cartascesions.Add(carta);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Carta de cesión subida correctamente.";
            return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
        }



        public async Task<IActionResult> DescargarCarta(int id)
        {
            var carta = await _context.Cartascesions.FindAsync(id);
            if (carta == null || string.IsNullOrWhiteSpace(carta.RutaArchivo) || !System.IO.File.Exists(carta.RutaArchivo))
            {
                return NotFound("Archivo no encontrado.");
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(carta.RutaArchivo);
            var tipo = carta.TipoMime ?? "application/octet-stream";
            var nombre = carta.NombreArchivo ?? $"Carta_{id}.pdf";

            return File(bytes, tipo, nombre);
        }


        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // GET: Cartascesion/Delete/5
        public async Task<IActionResult> Delete(int? id, int? integracionId)
        {
            if (id == null)
                return NotFound();

            var cartascesion = await _context.Cartascesions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cartascesion == null)
                return NotFound();

            ViewBag.IntegracionId = integracionId;
            return View(cartascesion);
        }


        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // POST: Cartascesion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int integracionId)
        {
            var carta = await _context.Cartascesions.FindAsync(id);
            if (carta != null)
            {
                if (!string.IsNullOrWhiteSpace(carta.RutaArchivo) && System.IO.File.Exists(carta.RutaArchivo))
                {
                    System.IO.File.Delete(carta.RutaArchivo);
                }

                _context.Cartascesions.Remove(carta);
                await _context.SaveChangesAsync();
            }

            TempData["Error"] = "Carta de cesión eliminada correctamente.";
            return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
        }
    }
}
