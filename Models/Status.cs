using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("statuses")]
[Index("Id", Name = "id_UNIQUE", IsUnique = true)]
public partial class Status
{
    [Key]
    public sbyte Id { get; set; }

    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime FechaCambio { get; set; }

    [InverseProperty("StatusNavigation")]
    public virtual ICollection<Integracione> Integraciones { get; set; } = new List<Integracione>();
}
