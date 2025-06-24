using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("cartascesion")]
public partial class Cartascesion
{
    [Key]
    public int Id { get; set; }

    [Column("CartasCesion")]
    public byte[] CartasCesion1 { get; set; } = null!;

    [InverseProperty("CartaCesionNavigation")]
    public virtual ICollection<Integracione> Integraciones { get; set; } = new List<Integracione>();
}
