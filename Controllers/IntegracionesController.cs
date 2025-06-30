using ElGantte.Data;
using ElGantte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGantte.Controllers
{
    [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
    public class IntegracionesController : Controller
    {
        private readonly AppDbContext _context;

        public IntegracionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Integraciones
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Integraciones.Include(i => i.PartnerNavigation).Include(i => i.SolucionNavigation).Include(i => i.StatusNavigation).Include(i => i.ModeloTerminalNavigation);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Integraciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var integracione = await _context.Integraciones
                .Include(i => i.PartnerNavigation)
                .Include(i => i.SolucionNavigation)
                .Include(i => i.StatusNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (integracione == null)
            {
                return NotFound();
            }

            return View(integracione);
        }

        // GET: Integraciones/Create
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View();
        }

        private async Task CargarListasAsync()
        {
            ViewBag.ModelosTerminales = new SelectList(await _context.Modelosterminals.ToListAsync(), "Id", "Modelo");
            ViewBag.Status = new SelectList(await _context.Statuses.ToListAsync(), "Id", "Nombre");
            ViewBag.Solucion = new SelectList(await _context.Soluciones.ToListAsync(), "Id", "Nombre");
            ViewBag.PartnerList = new SelectList(await _context.Partners.ToListAsync(), "Id", "Nombre");
        }


        // POST: Integraciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModeloTerminal,SoftwareIntegrado,NombreSwapp,Certificado,FechaInicio,FechaFin,DiasIntegrando,DiasStandBy,StandBy,CasoSf,Status,Solucion,Partner,CartaCesion")] Integracione integracione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(integracione);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Integración creada";
                return RedirectToAction(nameof(Index));
            }
            //Si falla la validación, necesitas recargar ViewBag
            await CargarListasAsync();

            TempData["Error"] = "Integración no guardada, datos incorrectos";
            return View(integracione);
        }

        // GET: Integraciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var integracione = await _context.Integraciones
                .Include(i => i.PartnerNavigation)
                .Include(i => i.SolucionNavigation)
                .Include(i => i.StatusNavigation)
                .Include(i => i.CartasCesion)
                .Include(i => i.CuadernosPrueba)
                .Include(i => i.TeleCertificaciones)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (integracione == null)
            {
                return NotFound();
            }
            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Nombre", integracione.Partner);
            ViewData["Solucion"] = new SelectList(_context.Soluciones, "Id", "Nombre", integracione.Solucion);
            ViewBag.ModelosTerminales = new SelectList(await _context.Modelosterminals.ToListAsync(), "Id", "Modelo");
            ViewData["Status"] = new SelectList(_context.Statuses, "Id", "Nombre", integracione.Status);
            return View(integracione);
        }

        // POST: Integraciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModeloTerminal,SoftwareIntegrado,NombreSwapp,Certificado,FechaInicio,FechaFin,DiasIntegrando,DiasStandBy,StandBy,CasoSf,Status,Solucion,Partner,CartaCesion")] Integracione integracione)
        {
            if (id != integracione.Id)
            {
                TempData["Error"] = "ID de integración no coincide";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(integracione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IntegracioneExists(integracione.Id))
                    {
                        TempData["Error"] = "Integración no encontrada";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Id", integracione.Partner);
            ViewData["Solucion"] = new SelectList(_context.Soluciones, "Id", "Id", integracione.Solucion);
            ViewData["Status"] = new SelectList(_context.Statuses, "Id", "Id", integracione.Status);
            TempData["Success"] = "Integración editada";
            return View(integracione);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // GET: Integraciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID de integración no coincide";
                return NotFound();
            }

            var integracione = await _context.Integraciones
                .Include(i => i.PartnerNavigation)
                .Include(i => i.SolucionNavigation)
                .Include(i => i.StatusNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (integracione == null)
            {
                TempData["Error"] = "Integración no encontrada";
                return NotFound();
            }

            return View(integracione);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // POST: Integraciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var integracione = await _context.Integraciones.FindAsync(id);
            if (integracione != null)
            {
                _context.Integraciones.Remove(integracione);
                TempData["Success"] = "Integración eliminada";
            } else
            {
                TempData["Error"] = "Integración no encontrada o ya eliminada";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IntegracioneExists(int id)
        {
            return _context.Integraciones.Any(e => e.Id == id);
        }
    }
}
