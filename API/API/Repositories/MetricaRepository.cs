using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Repositories
{
    public class MetricaRepository : IMetricaRepository
    {
        private readonly SkaL4bleContext _context;

        public MetricaRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public async Task<FuncionarioMetricaAtual?> ObterAtualAsync
            (Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return _context.FuncionarioMetricaAtual.FirstOrDefault(x => x.UsuarioId == usuarioId);
        }

        public async Task<List<FuncionarioMetricaAtual>>
            ListarAtuaisPorPeriodoAsync(
                DateTime periodoInicio,
                DateTime periodoFim,
                CancellationToken cancellationToken = default)
        {
            return await _context.FuncionarioMetricaAtual.Where(x =>
                    x.PeriodoInicio == periodoInicio &&
                    x.PeriodoFim == periodoFim)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HistoricoExisteAsync(
            Guid usuarioId,
            DateTime periodoInicio,
            DateTime periodoFim,
            CancellationToken cancellationToken = default)
        {
            return await _context.FuncionarioMetricaHistorico
                .AnyAsync(
                    x =>
                        x.UsuarioId == usuarioId &&
                        x.PeriodoInicio == periodoInicio &&
                        x.PeriodoFim == periodoFim,
                    cancellationToken);
        }

        public async Task<List<Ticket>>
            ListarTicketsDoPeriodoAsync(
                Guid usuarioId,
                DateTime periodoInicio,
                DateTime periodoFim,
                CancellationToken cancellationToken = default)
        {
            return await _context.Ticket
                .Include(x => x.StatusTicket)
                .Where(x =>
                    x.Ativo &&
                    x.UsuarioResponsavel == usuarioId &&
                    x.DataCriacao <= periodoFim &&
                    (
                        !x.DataEntrega.HasValue ||
                        x.DataEntrega.Value >= periodoInicio
                    ))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public void AdicionarAtual(
            FuncionarioMetricaAtual metrica)
        {
            _context.FuncionarioMetricaAtual.Add(metrica);
        }

        public void AdicionarHistorico(
            FuncionarioMetricaHistorico historico)
        {
            _context.FuncionarioMetricaHistorico.Add(historico);
        }

        public async Task<int> SalvarAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<IDbContextTransaction>
            IniciarTransacaoAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.Database.BeginTransactionAsync(
                cancellationToken);
        }
    }
}
