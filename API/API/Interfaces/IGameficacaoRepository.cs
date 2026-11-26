using API.Domains;

namespace API.Interfaces;

public interface IGamificacaoRepository
{
    Task<FuncionarioGamificacao?>
        ObterGamificacaoAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default);

    void AdicionarGamificacao(
        FuncionarioGamificacao gamificacao);

    Task<int> SalvarAsync(
        CancellationToken cancellationToken = default);
}