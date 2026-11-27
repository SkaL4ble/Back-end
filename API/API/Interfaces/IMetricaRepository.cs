using API.Domains;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Interfaces
{
    public interface IMetricaRepository
    {
        Task<FuncionarioMetrica?> ObterAtualAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default);

        Task<List<FuncionarioMetrica>> ListarAtuaisPorPeriodoAsync(
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
            FuncionarioMetrica metrica);

        void AdicionarHistorico(
            Log_FuncionarioMetrica historico);

        Task<int> SalvarAsync(
            CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> IniciarTransacaoAsync(
            CancellationToken cancellationToken = default);
    }
}
