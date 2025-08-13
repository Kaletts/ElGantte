using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("jiras")]
[Index("Id", Name = "id_UNIQUE", IsUnique = true)]
[Index("Partner", Name = "jiras_ibfk_1")]
public partial class Jira
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string? Codigo { get; set; }

    [Column(TypeName = "text")]
    public string? Descripcion { get; set; }

    public int? Partner { get; set; }

    [StringLength(255)]
    public string? Asunto { get; set; }

    [StringLength(255)]
    public string? Responsable { get; set; }

    public bool? Finalizado { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaFin { get; set; }

    [ForeignKey("Partner")]
    [InverseProperty("Jiras")]
    public virtual Partner? PartnerNavigation { get; set; }
}
