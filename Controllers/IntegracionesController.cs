﻿using ElGantte.Data;
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
                .Include(i => i.ModeloTerminalNavigation)
                .Include(i => i.Historicoetapas)
                .Include(i => i.Comentarios)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (integracione != null)
            {
                integracione.Comentarios = integracione.Comentarios
                    .OrderByDescending(c => c.Fecha)
                    .ToList();
            }

            if (integracione != null)
            {
                integracione.Historicoetapas = integracione.Historicoetapas
                    .OrderByDescending(c => c.FechaCambio)
                    .ToList();
            }

            //Obtener la última etapa (sin subetapa)
            var ultimaEtapa = integracione.Historicoetapas
                .Where(h => h.SubEtapa == null)
                .OrderByDescending(h => h.FechaCambio)
                .FirstOrDefault();

            //Obtener la última subetapa relacionada a esa etapa (si existe)
            int? ultimaSubEtapaId = null;
            if (ultimaEtapa != null)
            {
                var ultimaSubEtapa = integracione.Historicoetapas
                    .Where(h => h.SubEtapa == ultimaEtapa.Id)
                    .OrderByDescending(h => h.FechaCambio)
                    .FirstOrDefault();

                if (ultimaSubEtapa != null)
                {
                    ultimaSubEtapaId = ultimaSubEtapa.Id;
                }
            }

            ViewBag.SubEtapasDiccionario = await _context.Etapasintegracions
                .Where(e => e.Tipo == "Subetapa")
                .ToDictionaryAsync(e => e.Id, e => e.Nombre);

            if (integracione == null)
            {
                return NotFound();
            }

            //Cargar listas para selects filtrando por tipo
            var etapasNormales = await _context.Etapasintegracions
                .Where(e => e.Tipo == "Normal")
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            var subEtapas = await _context.Etapasintegracions
                .Where(e => e.Tipo == "Subetapa")
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            var model = _context.Integraciones
               .Include(i => i.Kitintegracions)
                   .ThenInclude(k => k.TarjetasNavigation)
               .Include(i => i.Kitintegracions)
                   .ThenInclude(k => k.TerminalNavigation)
                   .FirstOrDefault(i => i.Id == id);

            //Para la subetapa, se selecciona por Nombre si se tiene
            string ultimaSubEtapaNombre = null;
            if (ultimaSubEtapaId.HasValue)
            {
                var subEtapaEntity = subEtapas.FirstOrDefault(s => s.Id == ultimaSubEtapaId.Value);
                if (subEtapaEntity != null)
                {
                    ultimaSubEtapaNombre = subEtapaEntity.Nombre;
                }
            }
            ViewBag.SubEtapasDisponibles = new SelectList(subEtapas, "Id", "Nombre", ultimaSubEtapaNombre);
            ViewBag.TarjetasDisponibles = new SelectList(_context.Kittarjetas.ToList(), "Id", "Numero");
            ViewBag.TerminalesDisponibles = new SelectList(_context.Terminales.ToList(), "Id", "Serie");
            ViewBag.EtapasDisponibles = new SelectList(etapasNormales, "Nombre", "Nombre", ultimaEtapa?.Etapa);
            ViewBag.UltimaEtapaSeleccionada = ultimaEtapa?.Etapa;
            ViewBag.UltimaSubEtapaSeleccionada = ultimaSubEtapaNombre;


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
                    //Obtener el registro original para comparación
                    var original = await _context.Integraciones.AsNoTracking().FirstOrDefaultAsync(i => i.Id == integracione.Id);
                    if (original == null)
                    {
                        TempData["Error"] = "Integración no encontrada";
                        return NotFound();
                    }

                    //Asignar fechas de último día
                    if (original.StandBy.GetValueOrDefault() == false && integracione.StandBy.GetValueOrDefault() == true && integracione.Certificado.GetValueOrDefault() == false)
                    {
                        integracione.UltimoDiaIntegrando = DateTime.Today;
                        integracione.UltimoDiaStandBy = original.UltimoDiaStandBy;
                    }
                    else if (original.StandBy.GetValueOrDefault() == true && integracione.StandBy.GetValueOrDefault() == false && integracione.Certificado.GetValueOrDefault() == false)
                    {
                        integracione.UltimoDiaStandBy = DateTime.Today;
                        integracione.UltimoDiaIntegrando = original.UltimoDiaIntegrando;
                    }
                    else
                    {
                        integracione.UltimoDiaIntegrando = original.UltimoDiaIntegrando;
                        integracione.UltimoDiaStandBy = original.UltimoDiaStandBy;
                    }


                    _context.Update(integracione);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Integración editada";
                    return RedirectToAction(nameof(Index));
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
            }

            ViewData["Partner"] = new SelectList(_context.Partners, "Id", "Id", integracione.Partner);
            ViewData["Solucion"] = new SelectList(_context.Soluciones, "Id", "Id", integracione.Solucion);
            ViewData["Status"] = new SelectList(_context.Statuses, "Id", "Id", integracione.Status);
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
                .Include(i => i.ModeloTerminalNavigation)
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


        //Agregar comentario nuevo desde integracion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarComentario(int id, [Bind("Comentario1,Fecha,Integracion")] Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                var integracion = await _context.Integraciones.FindAsync(id);

                //Si por alguna razón el comentario no tiene fecha, se pone la actual
                if (comentario.Fecha == DateOnly.MinValue)
                {
                    comentario.Fecha = DateOnly.FromDateTime(DateTime.Today);
                }

                if (integracion != null)
                {
                    comentario.Integracion = integracion.Id; // Asociamos el comentario a la integración

                    //Guardo la fecha del comentario
                    DateOnly fechaBusqueda = comentario.Fecha;

                    //Busco el comentario existente previo
                    var comentarioExistente = await _context.Comentarios
                        .FirstOrDefaultAsync(c => c.Fecha == fechaBusqueda && c.Integracion == integracion.Id);

                    if (comentarioExistente != null)
                    {
                        //No hace falta escribir la fecha porque se debería conservar la original
                        comentarioExistente.Comentario1 = comentario.Comentario1;
                        _context.Update(comentarioExistente);
                        TempData["Warning"] = "Comentario actualizado";
                    }
                    else
                    {
                        TempData["Success"] = "Comentario creado";
                        _context.Add(comentario); //Agregamos el comentario a la base
                    }

                    await _context.SaveChangesAsync(); // Guardamos cambios
                }

                return RedirectToAction("Details", new { id = id });
            } else
            {
                TempData["Error"] = "Error creando el comentario";
            }

                //Si el modelo no es válido, vuelve a cargar la vista con el formulario y los datos necesarios
                return RedirectToAction("Details", new { id = id });
        }

        //Agregar nueva etapa desde integracion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarEtapa(int id, [Bind("FechaCambio,Etapa,Integracion")] Historicoetapa etapaElegida, int? subEtapa)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" | ", ModelState.SelectMany(
                    kvp => kvp.Value.Errors.Select(e => $"Campo: {kvp.Key} - {e.ErrorMessage}")
                ));
                return RedirectToAction("Details", new { id });
            }

            var integracion = await _context.Integraciones.FindAsync(id);
            if (integracion == null)
            {
                TempData["Error"] = "Integración no encontrada.";
                return RedirectToAction("Details", new { id });
            }

            etapaElegida.Integracion = id;

            if (etapaElegida.FechaCambio == DateOnly.MinValue)
            {
                etapaElegida.FechaCambio = DateOnly.FromDateTime(DateTime.Today);
            }

            // Guardamos el ID de la subetapa seleccionada (si aplica)
            etapaElegida.SubEtapa = subEtapa;

            // Verificamos si ya hay una etapa igual (solo si no es subetapa)
            if (!subEtapa.HasValue)
            {
                var etapaExistente = await _context.Historicoetapas
                    .FirstOrDefaultAsync(c => c.FechaCambio == etapaElegida.FechaCambio && c.Integracion == id && c.SubEtapa == null);

                if (etapaExistente != null)
                {
                    etapaExistente.Etapa = etapaElegida.Etapa;
                    _context.Update(etapaExistente);
                    TempData["Warning"] = "Etapa actualizada";
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });
                }
            }

            _context.Historicoetapas.Add(etapaElegida);
            await _context.SaveChangesAsync();

            TempData["Success"] = subEtapa.HasValue ? "Subetapa agregada correctamente." : "Etapa creada";
            return RedirectToAction("Details", new { id });
        }



        //Metodo para subetapas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarSubEtapa(int integracionId, int etapaPadreId, [Bind("FechaCambio,Etapa")] Historicoetapa subetapa)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos para subetapa.";
                return RedirectToAction("Details", new { id = integracionId });
            }

            var integracion = await _context.Integraciones.FindAsync(integracionId);
            var etapaPadre = await _context.Historicoetapas.FindAsync(etapaPadreId);

            if (integracion == null || etapaPadre == null)
            {
                TempData["Error"] = "Integración o etapa padre no encontrados.";
                return RedirectToAction("Details", new { id = integracionId });
            }

            if (subetapa.FechaCambio == DateOnly.MinValue)
            {
                subetapa.FechaCambio = DateOnly.FromDateTime(DateTime.Today);
            }

            subetapa.Integracion = integracionId;
            subetapa.SubEtapa = etapaPadreId; // Indica que es subetapa de la etapa padre

            _context.Historicoetapas.Add(subetapa);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Subetapa agregada correctamente.";
            return RedirectToAction("Details", new { id = integracionId });
        }


        //Método para obtener los datos del dashboard en formato JSON
        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var datos = new
            {
                integrando = _context.Integraciones.Count(i => i.Status == 1),
                standby = _context.Integraciones.Count(i => i.Status == 2),
                ko = _context.Integraciones.Count(i => i.Status == 3),
                certificado = _context.Integraciones.Count(i => i.Status == 4)
            };

            return Json(datos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarKitIntegracion(int id, Kitintegracion kit)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Formulario inválido.";
                return RedirectToAction("Details", new { id });
            }

            kit.Integracion = id;
            _context.Kitintegracions.Add(kit);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kit de integración guardado correctamente.";
            return RedirectToAction("Details", new { id });
        }


        private bool IntegracioneExists(int id)
        {
            return _context.Integraciones.Any(e => e.Id == id);
        }
    }
}
