using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElGantte.Models;

[Table("integraciones")]
[Index("CartaCesion", Name = "FK_CartaCesion_idx")]
[Index("Solucion", Name = "FK_Solucion_idx")]
[Index("Status", Name = "Status")]
[Index("Id", Name = "id_UNIQUE", IsUnique = true)]
[Index("Partner", Name = "integraciones_ibfk_2")]
public partial class Integracione
{
    [Key]
    public int Id { get; set; }

    [DisplayName("Modelo")]
    [StringLength(255)]
    public string ModeloTerminal { get; set; } = null!;

    [DisplayName("Software")]
    [StringLength(255)]
    public string SoftwareIntegrado { get; set; } = null!;

    [Column("NombreSWAPP")]
    [DisplayName("Nombre App")]
    [StringLength(255)]
    public string? NombreSwapp { get; set; }

    public bool? Certificado { get; set; }

    [DisplayName("Fecha Inicio")]
    public DateOnly? FechaInicio { get; set; }

    [DisplayName("Fecha Fin")]
    public DateOnly? FechaFin { get; set; }

    [DisplayName("Días Integrando")]
    public int? DiasIntegrando { get; set; }

    [DisplayName("Días Standby")]
    public int? DiasStandBy { get; set; }

    public bool? StandBy { get; set; }

    [DisplayName("Caso SF")]
    [Column("CasoSF")]
    [StringLength(255)]
    public string? CasoSf { get; set; }

    public sbyte? Status { get; set; }

    public int Solucion { get; set; }

    public int Partner { get; set; }

    public int? CartaCesion { get; set; }

    [ForeignKey("CartaCesion")]
    [InverseProperty("Integraciones")]
    public virtual Cartascesion? CartaCesionNavigation { get; set; }

    [InverseProperty("IntegracionNavigation")]
    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    [InverseProperty("IntegracionNavigation")]
    public virtual ICollection<Historicoetapa> Historicoetapas { get; set; } = new List<Historicoetapa>();

    [InverseProperty("IntegracionNavigation")]
    public virtual ICollection<Historicoreunione> Historicoreuniones { get; set; } = new List<Historicoreunione>();

    [InverseProperty("IntegracionNavigation")]
    public virtual ICollection<Kitintegracion> Kitintegracions { get; set; } = new List<Kitintegracion>();

    [DisplayName("Partner")]
    [ForeignKey("Partner")]
    [InverseProperty("Integraciones")]
    public virtual Partner PartnerNavigation { get; set; } = null!;

    [DisplayName("Solución")]
    [ForeignKey("Solucion")]
    [InverseProperty("Integraciones")]
    public virtual Solucione SolucionNavigation { get; set; } = null!;

    [DisplayName("Status")]
    [ForeignKey("Status")]
    [InverseProperty("Integraciones")]
    public virtual Status? StatusNavigation { get; set; }

    [InverseProperty("IntegracionNavigation")]
    public virtual ICollection<Terminale> Terminales { get; set; } = new List<Terminale>();
}
