using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElGantte.Models
{
    [Index(nameof(IntegracionId), IsUnique = false)]
    public class Kittarjeta
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Nombre { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public int? IntegracionId { get; set; }

        [ForeignKey("IntegracionId")]
        public virtual Integracione? Integracion { get; set; } = null!;

        public virtual ICollection<Tarjetas> Tarjetas { get; set; } = new List<Tarjetas>();
    }
}
