using ElGantte.Data;
using ElGantte.Migrations;
using ElGantte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Mime;

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
                .Select(i => new { Status = i.StatusNavigation != null ? i.StatusNavigation.Nombre : "Sin Estado" })
                .GroupBy(i => i.Status)
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

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult GetIntegracionesDetalle()
        {
            var data = _context.Integraciones
                .Include(i => i.PartnerNavigation)
                .Include(i => i.StatusNavigation)
                .Include(i => i.SolucionNavigation)
                .Include(i => i.ModeloTerminalNavigation)
                .Select(i => new
                {
                    Estado = i.StatusNavigation.Nombre,
                    Solucion = i.SolucionNavigation.Nombre,
                    Modelo = i.ModeloTerminalNavigation.Modelo,
                    Integrador = i.PartnerNavigation.Nombre,
                    StandBy = i.StandBy,
                    FechaInicio = i.FechaInicio.HasValue ? i.FechaInicio.Value.ToString("yyyy-MM-dd") : "Sin Fecha",
                    FechaFin = i.FechaFin.HasValue ? i.FechaFin.Value.ToString("yyyy-MM-dd") : "Sin Fecha"
                })
                .ToList();

            return Json(data);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult GetKPI()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var primerDiaMes = new DateOnly(hoy.Year, hoy.Month, 1);

            var integraciones = _context.Integraciones
                .Include(i => i.PartnerNavigation)
                .Include(i => i.StatusNavigation)
                .Include(i => i.ModeloTerminalNavigation)
                .Where(i => i.PartnerNavigation != null)
                .ToList();

            // Partners
            var partners = integraciones.Where(i => i.PartnerNavigation!.Tipo).ToList();
            var partnersMas60Dias = partners.Count(i => i.Certificado.GetValueOrDefault() && i.FechaInicio.HasValue && i.FechaFin.HasValue &&
                                                       (i.FechaFin.Value.DayNumber - i.FechaInicio.Value.DayNumber) > 60);
            var partnersMenos60Dias = partners.Count(i => i.Certificado.GetValueOrDefault() && i.FechaInicio.HasValue && i.FechaFin.HasValue &&
                                                          (i.FechaFin.Value.DayNumber - i.FechaInicio.Value.DayNumber) <= 60);
            var partnersIntegrandoMes = partners.Count(i => !i.Certificado.GetValueOrDefault() && !i.StandBy.GetValueOrDefault() &&
                                                            i.StatusNavigation?.Nombre?.Trim().Equals("Integración", StringComparison.OrdinalIgnoreCase) == true);
            var partnersCertificadosMes = partners.Count(i => i.Certificado.GetValueOrDefault() && i.FechaFin.HasValue && i.FechaFin.Value >= primerDiaMes);

            // Clientes
            var clientes = integraciones.Where(i => !i.PartnerNavigation!.Tipo).ToList();
            var clientesMas60Dias = clientes.Count(i => i.Certificado.GetValueOrDefault() && i.FechaInicio.HasValue && i.FechaFin.HasValue &&
                                                       (i.FechaFin.Value.DayNumber - i.FechaInicio.Value.DayNumber) > 60);
            var clientesMenos60Dias = clientes.Count(i => i.Certificado.GetValueOrDefault() && i.FechaInicio.HasValue && i.FechaFin.HasValue &&
                                                          (i.FechaFin.Value.DayNumber - i.FechaInicio.Value.DayNumber) <= 60);
            var clientesIntegrandoMes = clientes.Count(i => !i.Certificado.GetValueOrDefault() && !i.StandBy.GetValueOrDefault() &&
                                                            i.StatusNavigation?.Nombre?.Trim().Equals("Integración", StringComparison.OrdinalIgnoreCase) == true);
            var clientesCertificadosMes = clientes.Count(i => i.Certificado.GetValueOrDefault() && i.FechaFin.HasValue && i.FechaFin.Value >= primerDiaMes);

            var kpi = new
            {
                partnersMas60Dias,
                partnersMenos60Dias,
                partnersIntegrandoMes,
                partnersCertificadosMes,
                clientesMas60Dias,
                clientesMenos60Dias,
                clientesIntegrandoMes,
                clientesCertificadosMes
            };

            return Json(kpi);
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "User,Admin")]
        [HttpGet]
        public IActionResult ExportKPIsCSV()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var primerDiaMes = new DateOnly(hoy.Year, hoy.Month, 1);

            // Partners
            var partners = _context.Integraciones
                .Include(i => i.PartnerNavigation)
                .Include(i => i.ModeloTerminalNavigation)
                .Include(i => i.StatusNavigation)
                .Where(i => i.PartnerNavigation != null && i.PartnerNavigation.Tipo == true)
                .ToList();

            var partnersMas60Dias = partners
                .Where(i => i.Certificado == true
                            && i.FechaInicio.HasValue && i.FechaFin.HasValue
                            && (i.FechaFin.Value.DayNumber - i.FechaInicio.Value.DayNumber) > 60)
                .ToList();

            var partnersMenos60Dias = partners
                .Where(i => i.Certificado == true
                            && i.FechaInicio.HasValue && i.FechaFin.HasValue
                            && (i.FechaFin.Value.DayNumber - i.FechaInicio.Value.DayNumber) <= 60)
                .ToList();

            var partnersIntegrandoMes = partners
                .Where(i => i.StatusNavigation != null
                            && i.StatusNavigation.Nombre == "Integración"
                            && i.FechaInicio.HasValue && i.FechaInicio.Value.DayNumber >= primerDiaMes.DayNumber)
                .ToList();

            var partnersCertificadosMes = partners
                .Where(i => i.Certificado == true
                            && i.FechaFin.HasValue
                            && i.FechaFin.Value.DayNumber >= primerDiaMes.DayNumber)
                .ToList();

            // Clientes
            var clientes = _context.Integraciones
                .Include(i => i.PartnerNavigation)
                .Include(i => i.ModeloTerminalNavigation)
                .Include(i => i.StatusNavigation)
                .Where(i => i.PartnerNavigation != null && i.PartnerNavigation.Tipo == false)
                .ToList();

            var clientesMas60Dias = clientes
                .Where(i => i.Certificado == true
                            && i.FechaInicio.HasValue && i.FechaFin.HasValue
                            && (i.FechaFin.Value.DayNumber - i.FechaInicio.Value.DayNumber) > 60)
                .ToList();

            var clientesMenos60Dias = clientes
                .Where(i => i.Certificado == true
                            && i.FechaInicio.HasValue && i.FechaFin.HasValue
                            && (i.FechaFin.Value.DayNumber - i.FechaInicio.Value.DayNumber) <= 60)
                .ToList();

            var clientesIntegrandoMes = clientes
                .Where(i => i.StatusNavigation != null
                            && i.StatusNavigation.Nombre == "Integración"
                            && i.FechaInicio.HasValue && i.FechaInicio.Value.DayNumber >= primerDiaMes.DayNumber)
                .ToList();

            var clientesCertificadosMes = clientes
                .Where(i => i.Certificado == true
                            && i.FechaFin.HasValue
                            && i.FechaFin.Value.DayNumber >= primerDiaMes.DayNumber)
                .ToList();

            // Combinar todos en una lista para exportar
            var exportList = partnersMas60Dias
                .Concat(partnersMenos60Dias)
                .Concat(partnersIntegrandoMes)
                .Concat(partnersCertificadosMes)
                .Concat(clientesMas60Dias)
                .Concat(clientesMenos60Dias)
                .Concat(clientesIntegrandoMes)
                .Concat(clientesCertificadosMes)
                .Distinct() // Evitar duplicados
                .ToList();

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Tipo,Partner/Cliente,Modelo,Estado,Certificado,Fecha Inicio,Fecha Fin,Días StandBy,Días Integrando,Días Certificado");

            foreach (var i in exportList)
            {
                var tipo = i.PartnerNavigation.Tipo ? "Partner" : "Cliente";
                var nombre = i.PartnerNavigation.Nombre;
                var modelo = i.ModeloTerminalNavigation?.Modelo ?? "";
                var estado = i.StatusNavigation?.Nombre ?? "";
                var certificado = i.Certificado.GetValueOrDefault() ? "Sí" : "No";
                var fechaInicio = i.FechaInicio?.ToString("yyyy-MM-dd") ?? "";
                var fechaFin = i.FechaFin?.ToString("yyyy-MM-dd") ?? "";
                var diasStandBy = i.DiasStandBy.HasValue ? i.DiasStandBy.Value.ToString() : "0";
                var diasIntegrando = i.DiasIntegrando.HasValue ? i.DiasIntegrando.Value.ToString() : "0";
                var diasCertificado = (i.Certificado.GetValueOrDefault() && i.FechaInicio.HasValue && i.FechaFin.HasValue)
                                     ? (i.FechaFin.Value.DayNumber - i.FechaInicio.Value.DayNumber).ToString()
                                     : "";

                csv.AppendLine($"{tipo},{nombre},{modelo},{estado},{certificado},{fechaInicio},{fechaFin},{diasStandBy},{diasIntegrando},{diasCertificado}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            string fechaExport = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return File(bytes, "text/csv", $"KPIs_Integraciones-{fechaExport}.csv");
        }


    }
}
