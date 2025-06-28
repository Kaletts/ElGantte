using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElGantte.Models;

[Table("modelosterminal")]
[Index("Id", Name = "Id_UNIQUE", IsUnique = true)]
public partial class Modelosterminal
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Modelo { get; set; } = null!;

    [StringLength(50)]
    public string? Fabricante { get; set; }

    [InverseProperty("ModeloNavigation")]
    public virtual ICollection<Terminale> Terminales { get; set; } = new List<Terminale>();

    [InverseProperty("ModeloTerminalNavigation")]
    public virtual ICollection<Integracione> Integraciones { get; set; } = new List<Integracione>();

}
