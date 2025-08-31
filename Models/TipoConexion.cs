using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace ElGantte.Models
{
    [Index("Id", Name = "id_UNIQUE", IsUnique = true)]
    public partial class TipoConexion
    {
        [Key]
        public sbyte Id { get; set; }

        [StringLength(255)]
        public string Nombre { get; set; } = null!;

        [InverseProperty("TipoConexionNavigation")]
        public virtual ICollection<Integracione> Integraciones { get; set; } = new List<Integracione>();
    }
}