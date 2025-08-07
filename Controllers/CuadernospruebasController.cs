using ElGantte.Data;
using ElGantte.Migrations;
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
    public class CuadernospruebasController : Controller
    {
        private readonly AppDbContext _context;

        public CuadernospruebasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Cuadernospruebas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Cuadernosprueba.Include(c => c.Integracion);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Cuadernospruebas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuadernosprueba = await _context.Cuadernosprueba
                .Include(c => c.Integracion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuadernosprueba == null)
            {
                return NotFound();
            }

            return View(cuadernosprueba);
        }

        // GET: Cuadernospruebas/Create
        public IActionResult Create()
        {
            ViewData["IntegracioneId"] = new SelectList(_context.Integraciones, "Id", "Id");
            return View();
        }

        // POST: Cuadernospruebas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CartasCesion1,Fecha,NombreArchivo,TipoMime,IntegracioneId")] Cuadernosprueba cuadernosprueba)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cuadernosprueba);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IntegracioneId"] = new SelectList(_context.Integraciones, "Id", "Id", cuadernosprueba.IntegracioneId);
            return View(cuadernosprueba);
        }

        // GET: Cuadernospruebas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuadernosprueba = await _context.Cuadernosprueba.FindAsync(id);
            if (cuadernosprueba == null)
            {
                return NotFound();
            }
            ViewData["IntegracioneId"] = new SelectList(_context.Integraciones, "Id", "Id", cuadernosprueba.IntegracioneId);
            return View(cuadernosprueba);
        }

        //// POST: Cuadernospruebas/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,CartasCesion1,Fecha,NombreArchivo,TipoMime,IntegracioneId")] Cuadernosprueba cuadernosprueba)
        //{
        //    if (id != cuadernosprueba.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(cuadernosprueba);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CuadernospruebaExists(cuadernosprueba.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["IntegracioneId"] = new SelectList(_context.Integraciones, "Id", "Id", cuadernosprueba.IntegracioneId);
        //    return View(cuadernosprueba);
        //}

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // GET: Cuadernospruebas/Delete/5
        public async Task<IActionResult> Delete(int? id, int? integracionId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuadernosprueba = await _context.Cuadernosprueba
                .Include(c => c.Integracion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuadernosprueba == null)
            {
                return NotFound();
            }
            ViewBag.IntegracionId = integracionId;

            return View(cuadernosprueba);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // POST: Cuadernospruebas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int integracionId)
        {
            var cuadernosprueba = await _context.Cuadernosprueba.FindAsync(id);
            if (cuadernosprueba != null)
            {
                _context.Cuadernosprueba.Remove(cuadernosprueba);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Cuaderno de pruebas eliminado correctamente.";
            return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirCuaderno(IFormFile ArchivoCuadernoPrueba, int integracionId)
        {
            if (ArchivoCuadernoPrueba == null || ArchivoCuadernoPrueba.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo válido.";
                return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
            }

            var fileName = Path.GetFileName(ArchivoCuadernoPrueba.FileName);
            var folderPath = Path.Combine("/app/cuadernos", integracionId.ToString()); //Ruta DENTRO del contenedor
            Directory.CreateDirectory(folderPath); //Crea la carpeta si no existe

            var fullPath = Path.Combine(folderPath, fileName);

            // Guardar el archivo físico
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await ArchivoCuadernoPrueba.CopyToAsync(stream);
            }

            // Guardar metadata en BD
            var cuaderno = new Cuadernosprueba
            {
                RutaArchivo = fullPath,
                NombreArchivo = fileName,
                TipoMime = ArchivoCuadernoPrueba.ContentType,
                Fecha = DateTime.Today,
                IntegracioneId = integracionId
            };

            _context.Cuadernosprueba.Add(cuaderno);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cuaderno de prueba subido correctamente.";
            return RedirectToAction("Edit", "Integraciones", new { id = integracionId });
        }

        public async Task<IActionResult> Descargar(int id)
        {
            var cuaderno = await _context.Cuadernosprueba.FindAsync(id);
            if (cuaderno == null || string.IsNullOrWhiteSpace(cuaderno.RutaArchivo))
            {
                return NotFound();
            }

            if (!System.IO.File.Exists(cuaderno.RutaArchivo))
            {
                return NotFound("El archivo no existe en el sistema.");
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(cuaderno.RutaArchivo);
            var nombre = cuaderno.NombreArchivo ?? $"Cuaderno_{id}.xlsx";
            var tipo = cuaderno.TipoMime ?? "application/octet-stream";

            TempData["Success"] = "Cuaderno de pruebas descargado correctamente.";
            return File(bytes, tipo, nombre);
        }


    }
}
