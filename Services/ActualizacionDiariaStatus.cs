using ElGantte.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ElGantte.Services
{
    public class ActualizacionDiariaStatus : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ActualizacionDiariaStatus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await EjecutarActualizacionDiaria();

                //Esperar 24h para ejecutar la siguiente actualización
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task EjecutarActualizacionDiaria()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var integraciones = await context.Integraciones.ToListAsync();

            foreach (var integracion in integraciones)
            {
                DateTime hoy = DateTime.Today;

                if (integracion.StandBy == true &&
                    integracion.UltimoDiaStandBy.HasValue &&
                    integracion.UltimoDiaStandBy.Value.Date < hoy)
                {
                    //AddDays(1) Para evitar contar dos veces el mismo día (el día en que se actualizó por última vez). Así, solo se cuentan los días posteriores al último guardado.
                    int dias = DiasLaborables.CalcularDiasLaborales(integracion.UltimoDiaStandBy.Value.Date.AddDays(1), hoy);
                    if (dias > 0)
                    {
                        integracion.DiasStandBy += dias;
                        integracion.UltimoDiaStandBy = hoy;
                    }
                }


                if (integracion.StandBy == false &&
                    integracion.UltimoDiaIntegrando.HasValue &&
                    integracion.UltimoDiaIntegrando.Value.Date < hoy)
                {
                    int dias = DiasLaborables.CalcularDiasLaborales(integracion.UltimoDiaIntegrando.Value.Date.AddDays(1), hoy);
                    if (dias > 0)
                    {
                        integracion.DiasIntegrando += dias;
                        integracion.UltimoDiaIntegrando = hoy;
                    }
                }

            }

            await context.SaveChangesAsync();
        }
    }
    public static class DiasLaborables
    {
        public static List<DateTime> FestivosNacionales = new List<DateTime>
    {
        new DateTime(DateTime.Now.Year, 1, 1),   // Año Nuevo
        new DateTime(DateTime.Now.Year, 1, 6),   // Reyes
        new DateTime(DateTime.Now.Year, 5, 1),   // Día del Trabajo
        new DateTime(DateTime.Now.Year, 10, 12), // Fiesta Nacional
        new DateTime(DateTime.Now.Year, 12, 6),  // Constitución
        new DateTime(DateTime.Now.Year, 12, 25), // Navidad
    };

        public static int CalcularDiasLaborales(DateTime desde, DateTime hasta)
        {
            int dias = 0;
            DateTime fecha = desde.Date;

            while (fecha <= hasta.Date)
            {
                if (fecha.DayOfWeek != DayOfWeek.Saturday &&
                    fecha.DayOfWeek != DayOfWeek.Sunday &&
                    !FestivosNacionales.Contains(fecha))
                {
                    dias++;
                }
                fecha = fecha.AddDays(1);
            }

            return dias;
        }
    }

}
