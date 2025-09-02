using ElGantte.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElGantte.Models;

[Table("integraciones")]
[Index("Solucion", Name = "FK_Solucion_idx")]
[Index("Status", Name = "Status")]
[Index("Id", Name = "id_UNIQUE", IsUnique = true)]
[Index("Partner", Name = "integraciones_ibfk_2")]
public partial class Integracione
{
    [Key]
    public int Id { get; set; }

    [DisplayName("Modelo")]
    public int? ModeloTerminal { get; set; } = 1!;

    [DisplayName("Tipo Solución")]
    [StringLength(255)]
    public string SoftwareIntegrado { get; set; } = null!;

    [Column("NombreSWAPP")]
    [DisplayName("Aplicación")]
    [StringLength(255)]
    public string? NombreSwapp { get; set; }

    public bool? Certificado { get; set; } = false;

    [DisplayName("Fecha Inicio")]
    public DateOnly? FechaInicio { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [DisplayName("Fecha Fin")]
    public DateOnly? FechaFin { get; set; }

    [DisplayName("Días Integrando")]
    public int? DiasIntegrando { get; set; } = 0;

    [DisplayName("Días Standby")]
    public int? DiasStandBy { get; set; } = 0;

    public bool? StandBy { get; set; }

    [DisplayName("Caso SF")]
    [Column("CasoSF")]
    [StringLength(255)]
    public string? CasoSf { get; set; }

    public sbyte? Status { get; set; }

    public sbyte? Tipoconexion { get; set; }

    public int Solucion { get; set; }

    public int Partner { get; set; }

    [DisplayName("Último día en StandBy")]
    public DateTime? UltimoDiaStandBy { get; set; } = DateTime.Today;

    [DisplayName("Último día Integrando")]
    public DateTime? UltimoDiaIntegrando { get; set; } = DateTime.Today;


    [InverseProperty("Integracion")]
    public virtual ICollection<Cartascesion> CartasCesion { get; set; } = new List<Cartascesion>();

    [InverseProperty("Integracion")]
    public virtual ICollection<Cuadernosprueba> CuadernosPrueba { get; set; } = new List<Cuadernosprueba>();

    [InverseProperty("Integracion")]
    public virtual ICollection<Telecertificaciones> TeleCertificaciones { get; set; } = new List<Telecertificaciones>();

    [InverseProperty("IntegracionNavigation")]
    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    [InverseProperty("IntegracionNavigation")]
    public virtual ICollection<Historicoetapa> Historicoetapas { get; set; } = new List<Historicoetapa>();

    [InverseProperty("IntegracionNavigation")]
    public virtual ICollection<Historicoreunione> Historicoreuniones { get; set; } = new List<Historicoreunione>();

    [DisplayName("Partner")]
    [ForeignKey("Partner")]
    [InverseProperty("Integraciones")]
    public virtual Partner? PartnerNavigation { get; set; } = null!;

    [DisplayName("Solución")]
    [ForeignKey("Solucion")]
    [InverseProperty("Integraciones")]
    public virtual Solucione? SolucionNavigation { get; set; } = null!;

    [DisplayName("Status")]
    [ForeignKey("Status")]
    [InverseProperty("Integraciones")]
    public virtual Status? StatusNavigation { get; set; }

    [DisplayName("Tipo de Conexión")]
    [ForeignKey("Tipoconexion")]
    [InverseProperty("Integraciones")]
    public virtual Tipoconexion? TipoConexionNavigation { get; set; }
    public virtual ICollection<Terminale> Terminales { get; set; } = new List<Terminale>();

    public virtual ICollection<Kittarjeta> KitsTarjetas { get; set; } = new List<Kittarjeta>();

    [ForeignKey("ModeloTerminal")]
    [InverseProperty("Integraciones")]
    public Modelosterminal? ModeloTerminalNavigation { get; set; }

}
