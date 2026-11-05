using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class Skal4bleContext : DbContext
{
    public Skal4bleContext()
    {
    }

    public Skal4bleContext(DbContextOptions<Skal4bleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Atuacao> Atuacaos { get; set; }

    public virtual DbSet<Cargo> Cargos { get; set; }

    public virtual DbSet<ConsolidadoMetricaTecnologium> ConsolidadoMetricaTecnologia { get; set; }

    public virtual DbSet<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetricas { get; set; }

    public virtual DbSet<Projeto> Projetos { get; set; }

    public virtual DbSet<StatusProjeto> StatusProjetos { get; set; }

    public virtual DbSet<StatusTicket> StatusTickets { get; set; }

    public virtual DbSet<Tecnologium> Tecnologia { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TipoMetrica> TipoMetricas { get; set; }

    public virtual DbSet<TipoProjeto> TipoProjetos { get; set; }

    public virtual DbSet<TipoTicket> TipoTickets { get; set; }

    public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioEmailGit> UsuarioEmailGits { get; set; }

    public virtual DbSet<UsuarioIdeToken> UsuarioIdeTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Skal4ble;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atuacao>(entity =>
        {
            entity.ToTable("Atuacao");

            entity.HasIndex(e => e.NomeAtuacao, "UQ_Atuacao_Nome").IsUnique();

            entity.Property(e => e.NomeAtuacao)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.ToTable("Cargo");

            entity.HasIndex(e => e.NomeCargo, "UQ_Cargo_Nome").IsUnique();

            entity.Property(e => e.NomeCargo)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ConsolidadoMetricaTecnologium>(entity =>
        {
            entity.HasKey(e => e.ConsolidadoId);

            entity.HasIndex(e => new { e.UsuarioId, e.TecnologiaId, e.TipoMetricaId }, "UQ_Usuario_Tecnologia_Metrica").IsUnique();

            entity.Property(e => e.PontuacaoAtual).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UltimaAtualizacao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Tecnologia).WithMany(p => p.ConsolidadoMetricaTecnologia)
                .HasForeignKey(d => d.TecnologiaId)
                .HasConstraintName("FK_ConsTec_Tecnologia");

            entity.HasOne(d => d.TipoMetrica).WithMany(p => p.ConsolidadoMetricaTecnologia)
                .HasForeignKey(d => d.TipoMetricaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConsTec_TipoMetrica");

            entity.HasOne(d => d.Usuario).WithMany(p => p.ConsolidadoMetricaTecnologia)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_ConsTec_Usuario");
        });

        modelBuilder.Entity<HistoricoPontuacaoMetrica>(entity =>
        {
            entity.HasKey(e => e.HistoricoId);

            entity.ToTable("HistoricoPontuacaoMetrica");

            entity.Property(e => e.DataRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IncrementoAnterior).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Justificativa).IsUnicode(false);
            entity.Property(e => e.OrigemEvento)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ValorIncremento).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Tecnologia).WithMany(p => p.HistoricoPontuacaoMetricas)
                .HasForeignKey(d => d.TecnologiaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistPont_Tecnologia");

            entity.HasOne(d => d.TipoMetrica).WithMany(p => p.HistoricoPontuacaoMetricas)
                .HasForeignKey(d => d.TipoMetricaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistPont_TipoMetrica");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistoricoPontuacaoMetricas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_HistPont_Usuario");
        });

        modelBuilder.Entity<Projeto>(entity =>
        {
            entity.ToTable("Projeto", tb => tb.HasTrigger("TR_Projeto_SoftDeleteCascade"));

            entity.HasIndex(e => e.LinkRepositorio, "UQ_Projeto_LinkRepositorio").IsUnique();

            entity.Property(e => e.ProjetoId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.Descricao).IsUnicode(false);
            entity.Property(e => e.LinkRepositorio)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NomeProjeto)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.AutorNavigation).WithMany(p => p.Projetos)
                .HasForeignKey(d => d.Autor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projeto_Autor");

            entity.HasOne(d => d.StatusProjeto).WithMany(p => p.Projetos)
                .HasForeignKey(d => d.StatusProjetoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projeto_StatusProjeto");

            entity.HasOne(d => d.TipoProjeto).WithMany(p => p.Projetos)
                .HasForeignKey(d => d.TipoProjetoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projeto_TipoProjeto");

            entity.HasMany(d => d.Usuarios).WithMany(p => p.ProjetosNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjetoUsuario",
                    r => r.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioId")
                        .HasConstraintName("FK_ProjetoUsuario_Usuario"),
                    l => l.HasOne<Projeto>().WithMany()
                        .HasForeignKey("ProjetoId")
                        .HasConstraintName("FK_ProjetoUsuario_Projeto"),
                    j =>
                    {
                        j.HasKey("ProjetoId", "UsuarioId");
                        j.ToTable("ProjetoUsuario");
                    });
        });

        modelBuilder.Entity<StatusProjeto>(entity =>
        {
            entity.ToTable("StatusProjeto");

            entity.HasIndex(e => e.NomeStatusProjeto, "UQ_StatusProjeto_Nome").IsUnique();

            entity.Property(e => e.NomeStatusProjeto)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StatusTicket>(entity =>
        {
            entity.ToTable("StatusTicket");

            entity.HasIndex(e => e.NomeStatusTicket, "UQ_StatusTicket_Nome").IsUnique();

            entity.Property(e => e.NomeStatusTicket)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tecnologium>(entity =>
        {
            entity.HasKey(e => e.TecnologiaId);

            entity.HasIndex(e => e.NomeTecnologia, "UQ_Tecnologia_Nome").IsUnique();

            entity.Property(e => e.NomeTecnologia)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NomeTicket)
                .HasMaxLength(125)
                .IsUnicode(false);
            entity.Property(e => e.Prioridade).HasDefaultValue(1);

            entity.HasOne(d => d.Projeto).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ProjetoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Projeto");

            entity.HasOne(d => d.StatusTicket).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.StatusTicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_StatusTicket");

            entity.HasOne(d => d.TipoTicket).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.TipoTicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_TipoTicket");

            entity.HasOne(d => d.UsuarioResponsavelNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.UsuarioResponsavel)
                .HasConstraintName("FK_Ticket_UsuarioResponsavel");

            entity.HasMany(d => d.Colaboradors).WithMany(p => p.TicketsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "TicketColaborador",
                    r => r.HasOne<Usuario>().WithMany()
                        .HasForeignKey("ColaboradorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TickeColaborador_Tecnologia"),
                    l => l.HasOne<Ticket>().WithMany()
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TickeColaborador_Colaborador"),
                    j =>
                    {
                        j.HasKey("TicketId", "ColaboradorId");
                        j.ToTable("TicketColaborador");
                        j.IndexerProperty<int>("TicketId").ValueGeneratedOnAdd();
                    });

            entity.HasMany(d => d.Usuarios).WithMany(p => p.Tickets1)
                .UsingEntity<Dictionary<string, object>>(
                    "TicketUsuario",
                    r => r.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioId")
                        .HasConstraintName("FK_TicketUsuario_Usuario"),
                    l => l.HasOne<Ticket>().WithMany()
                        .HasForeignKey("TicketId")
                        .HasConstraintName("FK_TicketUsuario_Ticket"),
                    j =>
                    {
                        j.HasKey("TicketId", "UsuarioId").HasName("PK_TicketColaboradores");
                        j.ToTable("TicketUsuario");
                    });
        });

        modelBuilder.Entity<TipoMetrica>(entity =>
        {
            entity.ToTable("TipoMetrica");

            entity.HasIndex(e => e.NomeMetrica, "UQ_TipoMetrica_Nome").IsUnique();

            entity.Property(e => e.NomeMetrica)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoProjeto>(entity =>
        {
            entity.ToTable("TipoProjeto");

            entity.HasIndex(e => e.NomeTipoProjeto, "UQ_TipoProjeto_Nome").IsUnique();

            entity.Property(e => e.NomeTipoProjeto)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoTicket>(entity =>
        {
            entity.ToTable("TipoTicket");

            entity.HasIndex(e => e.NomeTipoTicket, "UQ_TipoTicket_Nome").IsUnique();

            entity.Property(e => e.NomeTipoTicket)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoUsuario>(entity =>
        {
            entity.ToTable("TipoUsuario");

            entity.HasIndex(e => e.NomeTipoUsuario, "UQ_TipoUsuario_Nome").IsUnique();

            entity.Property(e => e.TipoUsuarioId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.NomeTipoUsuario)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario", tb => tb.HasTrigger("TR_Usuario_SoftDeleteCascade"));

            entity.HasIndex(e => e.EmailGitHub, "UQ_Usuario_EmailGitHub").IsUnique();

            entity.HasIndex(e => e.EmailPessoal, "UQ_Usuario_EmailPessoal").IsUnique();

            entity.Property(e => e.UsuarioId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.EmailGitHub)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmailPessoal)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NomeUsuario)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Atuacao).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.AtuacaoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Atuacao");

            entity.HasOne(d => d.Cargo).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.CargoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Cargo");

            entity.HasOne(d => d.TipoUsuario).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.TipoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_TipoUsuario");

            entity.HasMany(d => d.Tecnologia).WithMany(p => p.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "TecnologiaUsuario",
                    r => r.HasOne<Tecnologium>().WithMany()
                        .HasForeignKey("TecnologiaId")
                        .HasConstraintName("FK_TecnologiaUsuario_Tecnologia"),
                    l => l.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioId")
                        .HasConstraintName("FK_TecnologiaUsuario_Usuario"),
                    j =>
                    {
                        j.HasKey("UsuarioId", "TecnologiaId");
                        j.ToTable("TecnologiaUsuario");
                    });
        });

        modelBuilder.Entity<UsuarioEmailGit>(entity =>
        {
            entity.ToTable("UsuarioEmailGit");

            entity.HasIndex(e => e.EmailGit, "UQ_UsuarioEmailGit_Email").IsUnique();

            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.EmailGit)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioEmailGits)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_UsuarioEmailGit_Usuario");
        });

        modelBuilder.Entity<UsuarioIdeToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

            entity.ToTable("UsuarioIdeToken");

            entity.Property(e => e.TokenId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApiKeyHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.NomeDispositivo)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioIdeTokens)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_UsuarioIdeToken_Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
