using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("historicoetapa")]
[Index("SubEtapa", Name = "FK_SubEtapa_idx")]
[Index("Integracion", Name = "Integracion")]
[Index("Id", Name = "id_UNIQUE", IsUnique = true)]
public partial class Historicoetapa
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public DateTime FechaCambio { get; set; }

    [StringLength(255)]
    public string Etapa { get; set; } = null!;

    public int? SubEtapa { get; set; }

    public int Integracion { get; set; }

    [ValidateNever]
    [ForeignKey("Integracion")]
    [InverseProperty("Historicoetapas")]
    public virtual Integracione IntegracionNavigation { get; set; } = null!;

    [InverseProperty("SubEtapaNavigation")]
    public virtual ICollection<Historicoetapa> InverseSubEtapaNavigation { get; set; } = new List<Historicoetapa>();

    [ForeignKey("SubEtapa")]
    [InverseProperty("InverseSubEtapaNavigation")]
    public virtual Historicoetapa? SubEtapaNavigation { get; set; }
}
