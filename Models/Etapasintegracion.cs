using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("etapasintegracion")]
[Index("Id", Name = "Id_UNIQUE", IsUnique = true)]
public partial class Etapasintegracion
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// Normal, Stand-by, KO
    /// </summary>
    [StringLength(50)]
    public string Tipo { get; set; } = null!;
}
