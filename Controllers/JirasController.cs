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
            return View(await appDbContext.ToListAsync());
        }

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
            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Id");
            return View();
        }

        // POST: Jiras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codigo,Descripcion,Partner,Asunto,Finalizado")] Jira jira)
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

        // POST: Jiras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Descripcion,Partner,Asunto,Finalizado")] Jira jira)
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

        private bool JiraExists(int id)
        {
            return _context.Jiras.Any(e => e.Id == id);
        }
    }
}
