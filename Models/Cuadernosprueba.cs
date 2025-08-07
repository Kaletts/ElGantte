using ElGantte.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElGantte.Models
{
    [Table("cuadernosprueba")]
    public partial class Cuadernosprueba
    {
        [Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string RutaArchivo { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Today;

        [StringLength(255)]
        public string NombreArchivo { get; set; }

        [StringLength(255)]
        public string TipoMime { get; set; }

        public int IntegracioneId { get; set; }

        [ForeignKey("IntegracioneId")]
        public virtual Integracione Integracion { get; set; } = null!;
    }
}