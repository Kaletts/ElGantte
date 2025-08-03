using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace ElGantte.Models
{
    [Table("tarjetas")]
    [Index("Id", Name = "id_UNIQUE", IsUnique = true)]
    public partial class Tarjetas
    {
        [Key]
        public sbyte Id { get; set; }
        public string Tipo { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string PIN { get; set; } = null!;
        public int KitAsociado { get; set; }

        [ForeignKey("KitAsociado")]
        [InverseProperty("Tarjetas")]
        public virtual int KitAsociadoNavigation { get; set; }

    }
}
