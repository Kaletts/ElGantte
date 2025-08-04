using System.ComponentModel.DataAnnotations;

namespace ElGantte.Models
{
    public class Kittarjeta
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Nombre { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Tarjetas> Tarjetas { get; set; } = new List<Tarjetas>();
    }
}
