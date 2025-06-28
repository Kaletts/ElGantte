using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElGantte.Models;

[Table("cartascesion")]
public partial class Cartascesion
{
    [Key]
    public int Id { get; set; }

    [Column("CartasCesion")]
    public byte[] CartasCesion1 { get; set; } = null!;

    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [StringLength(255)]
    public string NombreArchivo { get; set; }

    [StringLength(50)]
    public string TipoMime { get; set; }

    [InverseProperty("CartaCesionNavigation")]
    public virtual ICollection<Integracione> Integraciones { get; set; } = new List<Integracione>();
}
