using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElGantte.Models;

[Table("partners")]
[Index("Id", Name = "id_UNIQUE", IsUnique = true)]
public partial class Partner
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    [DisplayName("Dirección")]
    public string? Direccion { get; set; }

    public bool Tipo { get; set; }
    [DisplayName("Fecha Registro")]
    public DateOnly FechaRegistro { get; set; }

    [StringLength(255)]
    public string Correo { get; set; } = null!;

    [StringLength(20)]
    [DisplayName("Teléfono")]
    public string? Telefono { get; set; }

    [Column(TypeName = "text")]
    public string? Notas { get; set; }
    [DisplayName("Fecha Cambio")]
    public DateOnly? FechaCambio { get; set; }

    [StringLength(255)]
    [DisplayName("AM")]
    public string? AccountManager { get; set; }

    [InverseProperty("PartnerNavigation")]
    public virtual ICollection<Integracione> Integraciones { get; set; } = new List<Integracione>();

    [InverseProperty("PartnerNavigation")]
    public virtual ICollection<Jira> Jiras { get; set; } = new List<Jira>();
}
