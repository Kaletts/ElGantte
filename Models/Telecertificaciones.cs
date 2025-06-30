using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElGantte.Models
{
    [Table("telecertificaciones")]
    public partial class Telecertificaciones
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(500)")]
        [Required]
        public string Enlace { get; set; } = string.Empty;

        public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [MaxLength(1000)]
        public string Descripcion { get; set; } = string.Empty;
        
        public int IntegracioneId { get; set; }

        [ValidateNever]
        [ForeignKey("IntegracioneId")]
        public virtual Integracione Integracion { get; set; } = null!;
    }

}
