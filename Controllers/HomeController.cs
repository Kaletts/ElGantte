using ElGantte.Data;
using ElGantte.Migrations;
using ElGantte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ElGantte.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        public IActionResult XieGenerator()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetStatusSummary()
        {
            var data = await _context.Integraciones
                .Include(i => i.StatusNavigation)
                .GroupBy(i => i.StatusNavigation.Nombre)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Json(data);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetEtapasPorIntegracion()
        {
            var data = await _context.Historicoetapas
                .Include(h => h.IntegracionNavigation)
                .GroupBy(h => h.IntegracionNavigation.Id)
                .Select(g => new
                {
                    IntegracionId = g.Key,
                    Etapas = g
                        .OrderBy(e => e.FechaCambio)
                        .Select(e => e.Etapa)
                        .Distinct()
                        .ToList()
                })
                .ToListAsync();

            return Json(data);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetIntegracionesPorEtapaActual()
        {
            // Paso 1: Traer la última etapa de cada integración en memoria
            var etapasPorIntegracion = await _context.Historicoetapas
                .GroupBy(h => h.Integracion)
                .Select(g => g.OrderByDescending(h => h.FechaCambio).FirstOrDefault())
                .ToListAsync();

            // Paso 2: Agrupar en memoria por Etapa actual
            var resultado = etapasPorIntegracion
                .GroupBy(h => h.Etapa)
                .Select(g => new
                {
                    Etapa = g.Key,
                    Total = g.Count()
                })
                .ToList();

            return Json(resultado);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetIntegracionesPorSolucion()
        {
            var data = await _context.Integraciones
                .Include(i => i.SolucionNavigation)
                .GroupBy(i => i.SolucionNavigation.Nombre)
                .Select(g => new
                {
                    Solucion = g.Key ?? "Sin solución",
                    Count = g.Count()
                })
                .ToListAsync();

            return Json(data);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetIntegracionesPorEtapa(string etapa)
        {
            if (string.IsNullOrEmpty(etapa))
                return BadRequest();

            var integraciones = await _context.Historicoetapas
                .Where(h => h.Etapa == etapa)
                .Select(h => new
                {
                    h.Integracion,
                    Integrador = h.IntegracionNavigation.PartnerNavigation.Nombre,
                    Nombre = h.IntegracionNavigation.ModeloTerminalNavigation.Modelo

                })
                .Distinct()
                .ToListAsync();

            return Json(integraciones);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        public async Task<IActionResult> Informeservicio()
        {
            var integraciones = await _context.Integraciones
               .Include(i => i.PartnerNavigation)
               .Include(i => i.StatusNavigation)
               .Include(i => i.SolucionNavigation)
               .Include(i => i.ModeloTerminalNavigation)
               .Include(i => i.Comentarios)
               .Include(i => i.Historicoetapas)
               .ToListAsync();

            return View(integraciones);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult GetIntegracionesStandBy()
        {
            var integraciones = _context.Integraciones
                .Where(i => i.StandBy == true)
                .Select(i => new
                {
                    Modelo = i.ModeloTerminalNavigation.Modelo,
                    Integrador = i.PartnerNavigation.Nombre,
                    FechaInicio = i.FechaInicio.HasValue ? i.FechaInicio.Value.ToString("yyyy-MM-dd") : "Sin Fecha"
                })
                .ToList();

            return Json(integraciones);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult GetIntegracionesActivasPartners()
        {
            var integraciones = _context.Integraciones
                .Where(i => i.StandBy == false
                         && i.PartnerNavigation != null
                         && i.PartnerNavigation.Tipo == true)
                .Select(i => new
                {
                    Modelo = i.ModeloTerminalNavigation.Modelo,
                    Integrador = i.PartnerNavigation.Nombre,
                    FechaInicio = i.FechaInicio.HasValue ? i.FechaInicio.Value.ToString("yyyy-MM-dd") : "Sin Fecha"
                })
                .ToList();

            return Json(integraciones);
        }


        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult GetIntegracionesActivasClientes()
        {
            var integraciones = _context.Integraciones
                .Where(i => i.StandBy == false
                         && i.PartnerNavigation != null
                         && i.PartnerNavigation.Tipo == false)
                .Select(i => new
                {
                    Modelo = i.ModeloTerminalNavigation.Modelo,
                    Integrador = i.PartnerNavigation.Nombre,
                    FechaInicio = i.FechaInicio.HasValue ? i.FechaInicio.Value.ToString("yyyy-MM-dd") : "Sin Fecha"
                })
                .ToList();

            return Json(integraciones);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult GetIntegracionesActivasPartnersAno()
        {
            var integraciones = _context.Integraciones
                .Where(i => i.StandBy == false
                         && i.PartnerNavigation != null
                         && i.PartnerNavigation.Tipo == true
                         && i.FechaInicio.Value.Year == DateTime.Now.Year)
                .Select(i => new
                {
                    Modelo = i.ModeloTerminalNavigation.Modelo,
                    Integrador = i.PartnerNavigation.Nombre,
                    FechaInicio = i.FechaInicio.HasValue ? i.FechaInicio.Value.ToString("yyyy-MM-dd") : "Sin Fecha"
                })
                .ToList();

            return Json(integraciones);
        }


        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult GetIntegracionesActivasClientesAno()
        {
            var integraciones = _context.Integraciones
                .Where(i => i.StandBy == false
                         && i.PartnerNavigation != null
                         && i.PartnerNavigation.Tipo == false
                         && i.FechaInicio.Value.Year == DateTime.Now.Year)
                .Select(i => new
                {
                    Modelo = i.ModeloTerminalNavigation.Modelo,
                    Integrador = i.PartnerNavigation.Nombre,
                    FechaInicio = i.FechaInicio.HasValue ? i.FechaInicio.Value.ToString("yyyy-MM-dd") : "Sin Fecha"
                })
                .ToList();

            return Json(integraciones);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult GetCertificadosAno()
        {
            var integraciones = _context.Integraciones
                .Where(i => i.Certificado == true
                         && i.FechaFin.HasValue
                         && i.FechaFin.Value.Year == DateTime.Now.Year)
                .Select(i => new
                {
                    Modelo = i.ModeloTerminalNavigation.Modelo,
                    Integrador = i.PartnerNavigation.Nombre,
                    FechaFin = i.FechaFin.HasValue ? i.FechaFin.Value.ToString("yyyy-MM-dd") : "Sin Fecha"
                })
                .ToList();

            return Json(integraciones);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult GetCertificadosHistorico()
        {
            var integraciones = _context.Integraciones
                .Where(i => i.Certificado == true)
                .Select(i => new
                {
                    Modelo = i.ModeloTerminalNavigation.Modelo,
                    Integrador = i.PartnerNavigation.Nombre,
                    FechaFin = i.FechaFin.HasValue ? i.FechaFin.Value.ToString("yyyy-MM-dd") : "Sin Fecha"

                })
                .ToList();

            return Json(integraciones);
        }

    }
}
