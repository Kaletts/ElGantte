using ElGantte.Data;
using ElGantte.Models;
using ElGantte.Models.DTOs;
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
    public class JirasController : Controller
    {
        private readonly AppDbContext _context;

        public JirasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Jiras
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Jiras.Include(j => j.PartnerNavigation);
            var partners = await _context.Partners.ToListAsync();

            ViewBag.PartnerList = new SelectList(partners, "Id", "Nombre");
            ViewBag.Partners = partners;

            return View(await appDbContext.ToListAsync());
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // GET: Jiras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jira = await _context.Jiras
                .Include(j => j.PartnerNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jira == null)
            {
                return NotFound();
            }

            return View(jira);
        }

        // GET: Jiras/Create
        public IActionResult Create()
        {
            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Nombre");
            return View();
        }

        // POST: Jiras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codigo,Descripcion,Partner,Asunto,Responsable,Finalizado,FechaCreacion,FechaFin,Url")] Jira jira)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jira);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Id", jira.Partner);
            return View(jira);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // GET: Jiras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jira = await _context.Jiras.FindAsync(id);
            if (jira == null)
            {
                return NotFound();
            }
            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Id", jira.Partner);
            return View(jira);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // POST: Jiras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Descripcion,Partner,Asunto,Responsable,Finalizado,FechaCreacion,FechaFin,Url")] Jira jira)
        {
            if (id != jira.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jira);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JiraExists(jira.Id))
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
            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Id", jira.Partner);
            return View(jira);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // GET: Jiras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jira = await _context.Jiras
                .Include(j => j.PartnerNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jira == null)
            {
                return NotFound();
            }

            return View(jira);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        // POST: Jiras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jira = await _context.Jiras.FindAsync(id);
            if (jira != null)
            {
                _context.Jiras.Remove(jira);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRow([FromBody] JiraUpdateDto data)
        {
            if (data == null || data.Id == 0)
                return BadRequest();

            var jira = await _context.Jiras.FindAsync(data.Id);
            if (jira == null)
                return NotFound();

            foreach (var kvp in data.Values)
            {
                switch (kvp.Key)
                {
                    case "Codigo":
                        jira.Codigo = kvp.Value;
                        break;
                    case "Descripcion":
                        jira.Descripcion = kvp.Value;
                        break;
                    case "Asunto":
                        jira.Asunto = kvp.Value;
                        break;
                    case "Responsable":
                        jira.Responsable = kvp.Value;
                        break;
                    case "Finalizado":
                        jira.Finalizado = kvp.Value == "1";
                        break;
                    case "FechaCreacion":
                        if (DateTime.TryParse(kvp.Value, out var fechaCreacion))
                            jira.FechaCreacion = fechaCreacion;
                        break;

                    case "FechaFin":
                        if (DateTime.TryParse(kvp.Value, out var fechaFin))
                            jira.FechaFin = fechaFin;
                        else
                            jira.FechaFin = null;
                        break;

                    case "Url":
                        jira.Url = kvp.Value;
                        break;
                    case "Partner":
                        if (int.TryParse(kvp.Value, out var partnerId))
                            jira.Partner = partnerId;
                        break;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error guardando en DB: {ex.Message}");
            }
        }


        private bool JiraExists(int id)
        {
            return _context.Jiras.Any(e => e.Id == id);
        }
    }
}
