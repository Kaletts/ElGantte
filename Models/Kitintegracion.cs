using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("kitintegracion")]
[Index("Integracion", Name = "FK_Integracion_idx")]
[Index("Tarjetas", Name = "FK_Tarjetas_idx")]
[Index("Terminal", Name = "FK_Terminal_idx")]
public partial class Kitintegracion
{
    [Key]
    public int Id { get; set; }

    public int Integracion { get; set; }

    public sbyte Tarjetas { get; set; }

    public int? Terminal { get; set; }

    [ForeignKey("Integracion")]
    [InverseProperty("Kitintegracions")]
    public virtual Integracione IntegracionNavigation { get; set; } = null!;

    [ForeignKey("Tarjetas")]
    [InverseProperty("Kitintegracions")]
    public virtual Kittarjeta TarjetasNavigation { get; set; } = null!;

    [ForeignKey("Terminal")]
    [InverseProperty("Kitintegracions")]
    public virtual Terminale? TerminalNavigation { get; set; }
}
