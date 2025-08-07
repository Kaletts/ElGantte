using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElGantte.Models
{
    [Table("apikeys")]
    public class ApiKey
    {

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        public string Key { get; set; } = null!;

        [StringLength(50)]
        public string Nivel { get; set; } = null!;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public int PartnerId { get; set; }

        [ForeignKey(nameof(PartnerId))]
        public virtual Partner Partner { get; set; } = null!;
    }
}
