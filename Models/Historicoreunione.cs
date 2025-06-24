using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("historicoreuniones")]
[Index("Integracion", Name = "FK_Integraciones_idx")]
public partial class Historicoreunione
{
    [Key]
    public int Id { get; set; }

    public DateOnly? FechaReunion { get; set; }

    [StringLength(255)]
    public string? Enlace { get; set; }

    [Column(TypeName = "text")]
    public string? Acta { get; set; }

    public int? Integracion { get; set; }

    [ForeignKey("Integracion")]
    [InverseProperty("Historicoreuniones")]
    public virtual Integracione? IntegracionNavigation { get; set; }
}
