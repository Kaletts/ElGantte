using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("terminales")]
[Index("IntegracionId", Name = "FK_Integracion_idx")]
[Index("Modelo", Name = "FK_Modelo_idx")]
public partial class Terminale
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(90)]
    public string Serie { get; set; } = null!;
    public int Modelo { get; set; }
    public DateTime FechaUltimoCambio { get; set; } = DateTime.Now;
    public int? IntegracionId { get; set; } 

    [ForeignKey("IntegracionId")]
    [InverseProperty("Terminales")]
    public virtual Integracione? IntegracionNavigation { get; set; } = null!;

    [ForeignKey("Modelo")]
    [InverseProperty("Terminales")]
    public virtual Modelosterminal? ModeloNavigation { get; set; } = null!;
}
