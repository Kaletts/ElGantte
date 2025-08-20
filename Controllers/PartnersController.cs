using ElGantte.Data;
using ElGantte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ElGantte.Controllers
{
    [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
    public class PartnersController : Controller
    {
        private readonly AppDbContext _context;

        public PartnersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Partners
        public async Task<IActionResult> Index()
        {
            return View(await _context.Partners.ToListAsync());
        }

        // GET: Partners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partner = await _context.Partners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (partner == null)
            {
                return NotFound();
            }

            var integraciones = await _context.Integraciones
            .Include(i => i.ModeloTerminalNavigation)
            .Where(i => i.Partner == id)
            .ToListAsync();

            ViewBag.Integraciones = integraciones;

            return View(partner);
        }

        // GET: Partners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Partners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Direccion,Tipo,FechaRegistro,Correo,Telefono,Notas,FechaCambio,AccountManager")] Partner partner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(partner);
                partner.Correo = partner.Correo.Trim().ToLowerInvariant();
                partner.AccountManager = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(partner.AccountManager.Trim().ToLower());
                await _context.SaveChangesAsync();
                TempData["Success"] = "Partner creado";
                return RedirectToAction(nameof(Index));
            }
            return View(partner);
        }

        // GET: Partners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.TipoList = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Partner" },
                new SelectListItem { Value = "false", Text = "Cliente" }
            };


            var partner = await _context.Partners.FindAsync(id);
            if (partner == null)
            {
                return NotFound();
            }
            return View(partner);
        }

        // POST: Partners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Direccion,Tipo,FechaRegistro,Correo,Telefono,Notas,FechaCambio,AccountManager")] Partner partner)
        {
            if (id != partner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(partner);
                    TempData["Success"] = "Partner editado";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartnerExists(partner.Id))
                    {
                        TempData["Warning"] = "Partner no encontrado";
                        return NotFound();
                    }
                    else
                    {
                        TempData["Error"] = "No se pudo editar el Partner";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(partner);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // GET: Partners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partner = await _context.Partners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (partner == null)
            {
                return NotFound();
            }

            var integraciones = await _context.Integraciones
            .Where(i => i.Partner == id)
            .ToListAsync();

            ViewBag.Integraciones = integraciones;

            return View(partner);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // POST: Partners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner != null)
            {
                _context.Partners.Remove(partner);
                TempData["Success"] = "Partner eliminado";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PartnerExists(int id)
        {
            return _context.Partners.Any(e => e.Id == id);
        }
    }
}
