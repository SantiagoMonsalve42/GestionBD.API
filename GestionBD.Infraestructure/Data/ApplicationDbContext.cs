using System;
using System.Collections.Generic;
using GestionBD.Infraestructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Infraestructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblArtefacto> TblArtefactos { get; set; }

    public virtual DbSet<TblEjecucione> TblEjecuciones { get; set; }

    public virtual DbSet<TblEntregable> TblEntregables { get; set; }

    public virtual DbSet<TblInstancia> TblInstancias { get; set; }

    public virtual DbSet<TblLogEvento> TblLogEventos { get; set; }

    public virtual DbSet<TblLogTransaccione> TblLogTransacciones { get; set; }

    public virtual DbSet<TblMotore> TblMotores { get; set; }

    public virtual DbSet<TblParametro> TblParametros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // La configuración se hace desde Program.cs usando Dependency Injection
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblArtefacto>(entity =>
        {
            entity.HasKey(e => e.IdArtefacto);

            entity.ToTable("tbl_Artefactos");

            entity.Property(e => e.IdArtefacto)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idArtefacto");
            entity.Property(e => e.Codificacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codificacion");
            entity.Property(e => e.EsReverso).HasColumnName("esReverso");
            entity.Property(e => e.IdEntregable)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idEntregable");
            entity.Property(e => e.NombreArtefacto)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombreArtefacto");
            entity.Property(e => e.OrdenEjecucion).HasColumnName("ordenEjecucion");
            entity.Property(e => e.RutaRelativa)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("rutaRelativa");

            entity.HasOne(d => d.IdEntregableNavigation).WithMany(p => p.TblArtefactos)
                .HasForeignKey(d => d.IdEntregable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_Artefactos_tbl_Entregables");
        });

        modelBuilder.Entity<TblEjecucione>(entity =>
        {
            entity.HasKey(e => e.IdEjecucion).HasName("PK_tbl_Ejecuciones_1");

            entity.ToTable("tbl_Ejecuciones");

            entity.Property(e => e.IdEjecucion)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idEjecucion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.HoraFinEjecucion)
                .HasColumnType("datetime")
                .HasColumnName("horaFinEjecucion");
            entity.Property(e => e.HoraInicioEjecucion)
                .HasColumnType("datetime")
                .HasColumnName("horaInicioEjecucion");
            entity.Property(e => e.IdInstancia)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idInstancia");

            entity.HasOne(d => d.IdInstanciaNavigation).WithMany(p => p.TblEjecuciones)
                .HasForeignKey(d => d.IdInstancia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_Ejecuciones_tbl_Instancias1");
        });

        modelBuilder.Entity<TblEntregable>(entity =>
        {
            entity.HasKey(e => e.IdEntregable);

            entity.ToTable("tbl_Entregables");

            entity.Property(e => e.IdEntregable)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idEntregable");
            entity.Property(e => e.DescripcionEntregable)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("descripcionEntregable");
            entity.Property(e => e.IdEjecucion)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idEjecucion");
            entity.Property(e => e.RutaEntregable)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("rutaEntregable");

            entity.HasOne(d => d.IdEjecucionNavigation).WithMany(p => p.TblEntregables)
                .HasForeignKey(d => d.IdEjecucion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_Entregables_tbl_Ejecuciones");
        });

        modelBuilder.Entity<TblInstancia>(entity =>
        {
            entity.HasKey(e => e.IdInstancia);

            entity.ToTable("tbl_Instancias");

            entity.Property(e => e.IdInstancia)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idInstancia");
            entity.Property(e => e.IdMotor)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idMotor");
            entity.Property(e => e.Instancia)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("instancia");
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Puerto).HasColumnName("puerto");
            entity.Property(e => e.Usuario)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("usuario");

            entity.HasOne(d => d.IdMotorNavigation).WithMany(p => p.TblInstancia)
                .HasForeignKey(d => d.IdMotor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_Instancias_tbl_Motores");
        });

        modelBuilder.Entity<TblLogEvento>(entity =>
        {
            entity.HasKey(e => e.IdEvento);

            entity.ToTable("tbl_logEventos");

            entity.Property(e => e.IdEvento)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idEvento");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoEvento)
                .HasMaxLength(500)
                .HasColumnName("estadoEvento");
            entity.Property(e => e.FechaEjecucion)
                .HasColumnType("datetime")
                .HasColumnName("fechaEjecucion");
            entity.Property(e => e.IdTransaccion)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idTransaccion");

            entity.HasOne(d => d.IdTransaccionNavigation).WithMany(p => p.TblLogEventos)
                .HasForeignKey(d => d.IdTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tbl_logEventos_tbl_logTransacciones");
        });

        modelBuilder.Entity<TblLogTransaccione>(entity =>
        {
            entity.HasKey(e => e.IdTransaccion);

            entity.ToTable("tbl_logTransacciones");

            entity.Property(e => e.IdTransaccion)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idTransaccion");
            entity.Property(e => e.DescripcionTransaccion)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("descripcionTransaccion");
            entity.Property(e => e.EstadoTransaccion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("estadoTransaccion");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.NombreTransaccion)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombreTransaccion");
            entity.Property(e => e.RespuestaTransaccion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("respuestaTransaccion");
            entity.Property(e => e.UsuarioEjecucion)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("usuarioEjecucion");
        });

        modelBuilder.Entity<TblMotore>(entity =>
        {
            entity.HasKey(e => e.IdMotor);

            entity.ToTable("tbl_Motores");

            entity.Property(e => e.IdMotor)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idMotor");
            entity.Property(e => e.DescripcionMotor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcionMotor");
            entity.Property(e => e.NombreMotor)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombreMotor");
            entity.Property(e => e.VersionMotor)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("versionMotor");
        });

        modelBuilder.Entity<TblParametro>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbl_Parametros");

            entity.Property(e => e.IdParametro)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("idParametro");
            entity.Property(e => e.NombreParametro)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombreParametro");
            entity.Property(e => e.ValorNumerico)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("valorNumerico");
            entity.Property(e => e.ValorString)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("valorString");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
