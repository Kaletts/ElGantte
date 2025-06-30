using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ElGantte.Models;

[Table("comentarios")]
[Index("Integracion", Name = "Integracion")]
[Index("Id", Name = "id_UNIQUE", IsUnique = true)]
public partial class Comentario
{
    [Key]
    public int Id { get; set; }

    [Column("Comentario", TypeName = "text")]
    public string Comentario1 { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public int Integracion { get; set; }

    [ForeignKey("Integracion")]
    [InverseProperty("Comentarios")]
    [ValidateNever]
    public virtual Integracione IntegracionNavigation { get; set; } = null!;
}
