using System;
using System.Collections.Generic;
using API.Domains;
using Microsoft.EntityFrameworkCore;

namespace API.Contexts;

public partial class SkaL4bleContext : DbContext
{
    public SkaL4bleContext()
    {
    }

    public SkaL4bleContext(DbContextOptions<SkaL4bleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Atuacao> Atuacao { get; set; }

    public virtual DbSet<Cargo> Cargo { get; set; }

    public virtual DbSet<ConsolidadoMetricaTecnologia> ConsolidadoMetricaTecnologia { get; set; }

    public virtual DbSet<FuncionarioMetricaAtual> FuncionarioMetricaAtual { get; set; }

    public virtual DbSet<FuncionarioMetricaHistorico> FuncionarioMetricaHistorico { get; set; }

    public virtual DbSet<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetrica { get; set; }

    public virtual DbSet<Projeto> Projeto { get; set; }

    public virtual DbSet<ProjetoSonar> ProjetoSonar { get; set; }

    public virtual DbSet<StatusProjeto> StatusProjeto { get; set; }

    public virtual DbSet<StatusTicket> StatusTicket { get; set; }

    public virtual DbSet<Tecnologia> Tecnologia { get; set; }

    public virtual DbSet<Ticket> Ticket { get; set; }

    public virtual DbSet<TicketUsuario> TicketUsuario { get; set; }

    public virtual DbSet<TipoMetrica> TipoMetrica { get; set; }

    public virtual DbSet<TipoProjeto> TipoProjeto { get; set; }

    public virtual DbSet<TipoTicket> TipoTicket { get; set; }

    public virtual DbSet<TipoUsuario> TipoUsuario { get; set; }

    public virtual DbSet<TokenRedefinicaoSenha> TokenRedefinicaoSenha { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<UsuarioEmailGit> UsuarioEmailGit { get; set; }

    public virtual DbSet<UsuarioIdeToken> UsuarioIdeToken { get; set; }

    public virtual DbSet<Workflow> Workflow { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SkaL4ble;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atuacao>(entity =>
        {
            entity.HasIndex(e => e.NomeAtuacao, "UQ_Atuacao_Nome").IsUnique();

            entity.Property(e => e.NomeAtuacao)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasIndex(e => e.NomeCargo, "UQ_Cargo_Nome").IsUnique();

            entity.Property(e => e.NomeCargo)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ConsolidadoMetricaTecnologia>(entity =>
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

        modelBuilder.Entity<FuncionarioMetricaAtual>(entity =>
        {
            entity.HasKey(e => e.UsuarioId);

            entity.Property(e => e.UsuarioId).ValueGeneratedNever();
            entity.Property(e => e.DataAtualizacao).HasDefaultValueSql("(sysutcdatetime())", "DF_FMA_DataAtualizacao");
            entity.Property(e => e.NotaQuantidade).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.NotaVelocidade).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PontosQuantidade).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.QualidadeConfiabilidade).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QualidadeManutencao).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QualidadeSeguranca).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QualidadeSonar).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Usuario).WithOne(p => p.FuncionarioMetricaAtual)
                .HasForeignKey<FuncionarioMetricaAtual>(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FMA_Usuario");
        });

        modelBuilder.Entity<FuncionarioMetricaHistorico>(entity =>
        {
            entity.HasIndex(e => new { e.UsuarioId, e.PeriodoInicio, e.PeriodoFim }, "UQ_FMH_UsuarioPeriodo").IsUnique();

            entity.Property(e => e.DataFechamento).HasDefaultValueSql("(sysutcdatetime())", "DF_FMH_DataFechamento");
            entity.Property(e => e.NotaQualidade).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.NotaQuantidade).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.NotaVelocidade).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PontosQuantidade).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.QualidadeConfiabilidade).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QualidadeManutencao).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QualidadeSeguranca).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QualidadeSonar).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Usuario).WithMany(p => p.FuncionarioMetricaHistorico)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FMH_Usuario");
        });

        modelBuilder.Entity<HistoricoPontuacaoMetrica>(entity =>
        {
            entity.HasKey(e => e.HistoricoId);

            entity.Property(e => e.DataRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IncrementoAnterior).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Justificativa).IsUnicode(false);
            entity.Property(e => e.OrigemEvento)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ValorIncremento).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Tecnologia).WithMany(p => p.HistoricoPontuacaoMetrica)
                .HasForeignKey(d => d.TecnologiaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistPont_Tecnologia");

            entity.HasOne(d => d.TipoMetrica).WithMany(p => p.HistoricoPontuacaoMetrica)
                .HasForeignKey(d => d.TipoMetricaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistPont_TipoMetrica");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistoricoPontuacaoMetrica)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_HistPont_Usuario");
        });

        modelBuilder.Entity<Projeto>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("TR_Projeto_SoftDeleteCascade"));

            entity.HasIndex(e => e.LinkRepositorio, "UQ_Projeto_LinkRepositorio").IsUnique();

            entity.Property(e => e.ProjetoId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Descricao).IsUnicode(false);
            entity.Property(e => e.LinkRepositorio)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NomeProjeto)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.AutorNavigation).WithMany(p => p.Projeto)
                .HasForeignKey(d => d.Autor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projeto_Autor");

            entity.HasOne(d => d.StatusProjeto).WithMany(p => p.Projeto)
                .HasForeignKey(d => d.StatusProjetoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projeto_StatusProjeto");

            entity.HasOne(d => d.TipoProjeto).WithMany(p => p.Projeto)
                .HasForeignKey(d => d.TipoProjetoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projeto_TipoProjeto");

            entity.HasMany(d => d.Usuario).WithMany(p => p.ProjetoNavigation)
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
                    });
        });

        modelBuilder.Entity<ProjetoSonar>(entity =>
        {
            entity.HasIndex(e => e.ChaveSonar, "UQ_ProjetoSonar_ChaveSonar").IsUnique();

            entity.Property(e => e.ProjetoSonarId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ChaveSonar)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Descricao)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.NomeSonar)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Origem)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TokenSonarCriptografado).IsUnicode(false);
            entity.Property(e => e.WorkflowId).HasDefaultValue(1);

            entity.HasOne(d => d.Projeto).WithMany(p => p.ProjetoSonar)
                .HasForeignKey(d => d.ProjetoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjetoSonar_Projeto");
        });

        modelBuilder.Entity<StatusProjeto>(entity =>
        {
            entity.HasIndex(e => e.NomeStatusProjeto, "UQ_StatusProjeto_Nome").IsUnique();

            entity.Property(e => e.NomeStatusProjeto)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StatusTicket>(entity =>
        {
            entity.HasIndex(e => e.NomeStatusTicket, "UQ_StatusTicket_Nome").IsUnique();

            entity.Property(e => e.NomeStatusTicket)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tecnologia>(entity =>
        {
            entity.HasIndex(e => e.NomeTecnologia, "UQ_Tecnologia_Nome").IsUnique();

            entity.Property(e => e.NomeTecnologia)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NomeTicket)
                .HasMaxLength(125)
                .IsUnicode(false);
            entity.Property(e => e.Prioridade).HasDefaultValue(1);

            entity.HasOne(d => d.Projeto).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.ProjetoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Projeto");

            entity.HasOne(d => d.StatusTicket).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.StatusTicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_StatusTicket");

            entity.HasOne(d => d.TipoTicket).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.TipoTicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_TipoTicket");

            entity.HasOne(d => d.UsuarioResponsavelNavigation).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.UsuarioResponsavel)
                .HasConstraintName("FK_Ticket_UsuarioResponsavel");
        });

        modelBuilder.Entity<TicketUsuario>(entity =>
        {
            entity.HasKey(e => new { e.TicketId, e.UsuarioId }).HasName("PK_TicketColaboradores");

            entity.Property(e => e.Autor).HasDefaultValue(true);

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketUsuario)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK_TicketUsuario_Ticket");

            entity.HasOne(d => d.Usuario).WithMany(p => p.TicketUsuario)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_TicketUsuario_Usuario");
        });

        modelBuilder.Entity<TipoMetrica>(entity =>
        {
            entity.HasIndex(e => e.NomeMetrica, "UQ_TipoMetrica_Nome").IsUnique();

            entity.Property(e => e.NomeMetrica)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoProjeto>(entity =>
        {
            entity.HasIndex(e => e.NomeTipoProjeto, "UQ_TipoProjeto_Nome").IsUnique();

            entity.Property(e => e.NomeTipoProjeto)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoTicket>(entity =>
        {
            entity.HasIndex(e => e.NomeTipoTicket, "UQ_TipoTicket_Nome").IsUnique();

            entity.Property(e => e.NomeTipoTicket)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoUsuario>(entity =>
        {
            entity.HasIndex(e => e.CodigoTipoUsuario, "UQ_TipoUsuario_Codigo").IsUnique();

            entity.HasIndex(e => e.NomeTipoUsuario, "UQ_TipoUsuario_Nome").IsUnique();

            entity.Property(e => e.TipoUsuarioId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CodigoTipoUsuario)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.NomeTipoUsuario)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TokenRedefinicaoSenha>(entity =>
        {
            entity.HasIndex(e => e.UsuarioId, "IX_TokenRedefinicaoSenha_UsuarioId");

            entity.HasIndex(e => e.TokenHash, "UQ_TokenRedefinicaoSenha_TokenHash").IsUnique();

            entity.Property(e => e.TokenHash)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Usuario).WithMany(p => p.TokenRedefinicaoSenha)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TokenRedefinicaoSenha_Usuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("TR_Usuario_SoftDeleteCascade"));

            entity.HasIndex(e => e.EmailPessoal, "UQ_Usuario_EmailPessoal").IsUnique();

            entity.Property(e => e.UsuarioId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.EmailPessoal)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NomeUsuario)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Atuacao).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.AtuacaoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Atuacao");

            entity.HasOne(d => d.Cargo).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.CargoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Cargo");

            entity.HasOne(d => d.TipoUsuario).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.TipoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_TipoUsuario");

            entity.HasMany(d => d.Tecnologia).WithMany(p => p.Usuario)
                .UsingEntity<Dictionary<string, object>>(
                    "TecnologiaUsuario",
                    r => r.HasOne<Tecnologia>().WithMany()
                        .HasForeignKey("TecnologiaId")
                        .HasConstraintName("FK_TecnologiaUsuario_Tecnologia"),
                    l => l.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioId")
                        .HasConstraintName("FK_TecnologiaUsuario_Usuario"),
                    j =>
                    {
                        j.HasKey("UsuarioId", "TecnologiaId");
                    });
        });

        modelBuilder.Entity<UsuarioEmailGit>(entity =>
        {
            entity.HasIndex(e => e.EmailGit, "UQ_UsuarioEmailGit_Email").IsUnique();

            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.EmailGit)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioEmailGit)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_UsuarioEmailGit_Usuario");
        });

        modelBuilder.Entity<UsuarioIdeToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

            entity.Property(e => e.TokenId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApiKeyHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.NomeDispositivo)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioIdeToken)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_UsuarioIdeToken_Usuario");
        });

        modelBuilder.Entity<Workflow>(entity =>
        {
            entity.HasIndex(e => e.NomeWorkflow, "UQ_Workflow_Nome").IsUnique();

            entity.Property(e => e.WorkflowId).ValueGeneratedNever();
            entity.Property(e => e.NomeWorkflow)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
