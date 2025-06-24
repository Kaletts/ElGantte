using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("kittarjetas")]
[Index("Id", Name = "id_UNIQUE", IsUnique = true)]
public partial class Kittarjeta
{
    [Key]
    public sbyte Id { get; set; }

    [StringLength(50)]
    public string Numero { get; set; } = null!;

    [StringLength(50)]
    public string Tipo { get; set; } = null!;

    [Column("PIN")]
    public short? Pin { get; set; }

    [InverseProperty("TarjetasNavigation")]
    public virtual ICollection<Kitintegracion> Kitintegracions { get; set; } = new List<Kitintegracion>();
}
