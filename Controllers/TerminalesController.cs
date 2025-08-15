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
    public class TerminalesController : Controller
    {
        private readonly AppDbContext _context;

        public TerminalesController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Terminales
        // GET: Terminales
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Terminales
                .Include(t => t.IntegracionNavigation)
                    .ThenInclude(i => i.SolucionNavigation)
                .Include(t => t.IntegracionNavigation)
                    .ThenInclude(i => i.StatusNavigation)
                .Include(t => t.IntegracionNavigation)
                    .ThenInclude(i => i.PartnerNavigation)
                .Include(t => t.ModeloNavigation);

            ViewData["Modelo"] = new SelectList(_context.Modelosterminals, "Id", "Modelo");

            return View(await appDbContext.ToListAsync());
        }


        // GET: Terminales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terminale = await _context.Terminales
                .Include(t => t.IntegracionNavigation)
                .Include(t => t.ModeloNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (terminale == null)
            {
                return NotFound();
            }

            return View(terminale);
        }

        // GET: Terminales/Create
        public IActionResult Create()
        {
            ViewData["IntegracionId"] = new SelectList(_context.Integraciones, "Id", "Id");
            ViewData["Modelo"] = new SelectList(_context.Modelosterminals, "Id", "Id");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Serie,Modelo")] Terminale terminale)
        {
            if (ModelState.IsValid)
            {
                terminale.FechaUltimoCambio = DateTime.Now; // asigna la fecha actual
                terminale.IntegracionId = null; // sin integración al crearlo

                _context.Add(terminale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Modelo"] = new SelectList(_context.Modelosterminals, "Id", "Nombre", terminale.Modelo);
            return View(terminale);
        }


        // GET: Terminales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terminale = await _context.Terminales.FindAsync(id);
            if (terminale == null)
            {
                return NotFound();
            }

            ViewData["IntegracionId"] = new SelectList(
                _context.Integraciones
                    .Select(i => new
                    {
                        i.Id,
                        Text = i.Id + " (" + (i.PartnerNavigation != null ? i.PartnerNavigation.Nombre : "Sin partner") + ")"
                    }),
                "Id",
                "Text",
                terminale.IntegracionId
            );

            ViewData["Modelo"] = new SelectList(_context.Modelosterminals, "Id", "Modelo", terminale.Modelo);
            return View(terminale);
        }

        // POST: Terminales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Serie,Modelo,IntegracionId")] Terminale terminale)
        {
            if (id != terminale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el terminal existente desde la base de datos
                    var terminalExistente = await _context.Terminales.FindAsync(id);
                    if (terminalExistente == null)
                        return NotFound();

                    // Actualizar solo los campos que vienen del formulario
                    terminalExistente.Serie = terminale.Serie;
                    terminalExistente.Modelo = terminale.Modelo;
                    terminalExistente.IntegracionId = terminale.IntegracionId; // puede ser null

                    // Actualizar la fecha automáticamente
                    terminalExistente.FechaUltimoCambio = DateTime.Now;

                    _context.Update(terminalExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerminaleExists(terminale.Id))
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

            ViewData["IntegracionId"] = new SelectList(_context.Integraciones, "Id", "Id", terminale.IntegracionId);
            ViewData["Modelo"] = new SelectList(_context.Modelosterminals, "Id", "Id", terminale.Modelo);
            return View(terminale);
        }


        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // GET: Terminales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terminale = await _context.Terminales
                .Include(t => t.IntegracionNavigation)
                .Include(t => t.ModeloNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (terminale == null)
            {
                return NotFound();
            }

            return View(terminale);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // POST: Terminales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var terminale = await _context.Terminales.FindAsync(id);
            if (terminale != null)
            {
                _context.Terminales.Remove(terminale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TerminaleExists(int id)
        {
            return _context.Terminales.Any(e => e.Id == id);
        }
    }
}
