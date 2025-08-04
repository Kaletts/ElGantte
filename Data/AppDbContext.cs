using ElGantte.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using System;
using System.Collections.Generic;

namespace ElGantte.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cartascesion> Cartascesions { get; set; }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<Etapasintegracion> Etapasintegracions { get; set; }

    public virtual DbSet<Historicoetapa> Historicoetapas { get; set; }

    public virtual DbSet<Historicoreunione> Historicoreuniones { get; set; }

    public virtual DbSet<Integracione> Integraciones { get; set; }

    public virtual DbSet<Jira> Jiras { get; set; }

    public virtual DbSet<Modelosterminal> Modelosterminals { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Solucione> Soluciones { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Terminale> Terminales { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Kittarjeta> KitTarjetas { get; set; }

    public virtual DbSet<Tarjetas> Tarjetas { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("server=localhost;user=root;password=YaTuSabes25%?;database=lbase2", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cartascesion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.IntegracionNavigation).WithMany(p => p.Comentarios).HasConstraintName("comentarios_ibfk_1");
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Etapasintegracion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Tipo).HasComment("Normal, Stand-by, KO");
        });

        modelBuilder.Entity<Historicoetapa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IntegracionNavigation).WithMany(p => p.Historicoetapas).HasConstraintName("historicoetapa_ibfk_1");

            entity.HasOne(d => d.SubEtapaNavigation).WithMany(p => p.InverseSubEtapaNavigation).HasConstraintName("FK_SubEtapa");
        });

        modelBuilder.Entity<Historicoreunione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.IntegracionNavigation).WithMany(p => p.Historicoreuniones).HasConstraintName("FK_Integraciones");
        });

        modelBuilder.Entity<Integracione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.PartnerNavigation).WithMany(p => p.Integraciones).HasConstraintName("integraciones_ibfk_2");

            entity.HasOne(d => d.SolucionNavigation).WithMany(p => p.Integraciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solucion");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Integraciones)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("integraciones_ibfk_1");
        });

        modelBuilder.Entity<Integracione>()
                .HasOne(i => i.ModeloTerminalNavigation)
                .WithMany(mt => mt.Integraciones)
                .HasForeignKey(i => i.ModeloTerminal);

        modelBuilder.Entity<Jira>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.PartnerNavigation).WithMany(p => p.Jiras)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("jiras_ibfk_1");
        });

        modelBuilder.Entity<Modelosterminal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Solucione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Terminale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).ValueGeneratedNever();

            //entity.HasOne(d => d.IntegracionNavigation).WithMany(p => p.Terminales).HasConstraintName("FK_Integracion_M");

            entity.HasOne(d => d.ModeloNavigation).WithMany(p => p.Terminales)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Modelo");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public DbSet<ElGantte.Models.Cuadernosprueba> Cuadernosprueba { get; set; } = default!;

    public DbSet<ElGantte.Models.Telecertificaciones> Telecertificaciones { get; set; } = default!;
}
