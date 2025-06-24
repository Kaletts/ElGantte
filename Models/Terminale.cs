using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("terminales")]
[Index("Integracion", Name = "FK_Integracion_idx")]
[Index("Modelo", Name = "FK_Modelo_idx")]
public partial class Terminale
{
    [Key]
    public int Id { get; set; }

    [StringLength(45)]
    public string Serie { get; set; } = null!;

    public int Modelo { get; set; }

    public int? Integracion { get; set; }

    [ForeignKey("Integracion")]
    [InverseProperty("Terminales")]
    public virtual Integracione? IntegracionNavigation { get; set; }

    [InverseProperty("TerminalNavigation")]
    public virtual ICollection<Kitintegracion> Kitintegracions { get; set; } = new List<Kitintegracion>();

    [ForeignKey("Modelo")]
    [InverseProperty("Terminales")]
    public virtual Modelosterminal ModeloNavigation { get; set; } = null!;
}
