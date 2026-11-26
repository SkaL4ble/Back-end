using API.Domains;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Interfaces
{
    public interface IMetricaRepository
    {
        Task<FuncionarioMetricaAtual?> ObterAtualAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default);

        Task<List<FuncionarioMetricaAtual>> ListarAtuaisPorPeriodoAsync(
            DateTime periodoInicio,
            DateTime periodoFim,
            CancellationToken cancellationToken = default);

        Task<bool> HistoricoExisteAsync(
            Guid usuarioId,
            DateTime periodoInicio,
            DateTime periodoFim,
            CancellationToken cancellationToken = default);

        Task<List<Ticket>> ListarTicketsDoPeriodoAsync(
            Guid usuarioId,
            DateTime periodoInicio,
            DateTime periodoFim,
            CancellationToken cancellationToken = default);

        void AdicionarAtual(
            FuncionarioMetricaAtual metrica);

        void AdicionarHistorico(
            FuncionarioMetricaHistorico historico);

        Task<int> SalvarAsync(
            CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> IniciarTransacaoAsync(
            CancellationToken cancellationToken = default);
    }
}
