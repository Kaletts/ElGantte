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

        [Column("CuadernoPrueba")]
        public byte[] CuadernoPrueba { get; set; } = null!;
        public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [StringLength(255)]
        public string NombreArchivo { get; set; }

        [StringLength(50)]
        public string TipoMime { get; set; }

        public int IntegracioneId { get; set; }

        [ForeignKey("IntegracioneId")]
        public virtual Integracione Integracion { get; set; } = null!;
    }
}