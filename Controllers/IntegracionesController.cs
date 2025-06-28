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
            var appDbContext = _context.Integraciones.Include(i => i.CartaCesionNavigation).Include(i => i.PartnerNavigation).Include(i => i.SolucionNavigation).Include(i => i.StatusNavigation).Include(i => i.ModeloTerminalNavigation);
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
                .Include(i => i.CartaCesionNavigation)
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
                return RedirectToAction(nameof(Index));
            }
            //Si falla la validación, necesitas recargar ViewBag
            await CargarListasAsync();
            return View(integracione);
        }

        // GET: Integraciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var integracione = await _context.Integraciones.FindAsync(id);
            if (integracione == null)
            {
                return NotFound();
            }
            ViewData["CartaCesion"] = new SelectList(_context.Cartascesions, "Id", "Id", integracione.CartaCesion);
            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Id", integracione.Partner);
            ViewData["Solucion"] = new SelectList(_context.Soluciones, "Id", "Id", integracione.Solucion);
            ViewData["Status"] = new SelectList(_context.Statuses, "Id", "Id", integracione.Status);
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
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CartaCesion"] = new SelectList(_context.Cartascesions, "Id", "Id", integracione.CartaCesion);
            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Id", integracione.Partner);
            ViewData["Solucion"] = new SelectList(_context.Soluciones, "Id", "Id", integracione.Solucion);
            ViewData["Status"] = new SelectList(_context.Statuses, "Id", "Id", integracione.Status);
            return View(integracione);
        }

        // GET: Integraciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var integracione = await _context.Integraciones
                .Include(i => i.CartaCesionNavigation)
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

        // POST: Integraciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var integracione = await _context.Integraciones.FindAsync(id);
            if (integracione != null)
            {
                _context.Integraciones.Remove(integracione);
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
