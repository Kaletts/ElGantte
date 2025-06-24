using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("soluciones")]
[Index("Id", Name = "Id_UNIQUE", IsUnique = true)]
public partial class Solucione
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [StringLength(100)]
    public string? Descripcion { get; set; }

    [InverseProperty("SolucionNavigation")]
    public virtual ICollection<Integracione> Integraciones { get; set; } = new List<Integracione>();
}
