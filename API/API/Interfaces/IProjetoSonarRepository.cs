using API.Domains;

namespace API.Interfaces;

public interface IProjetoSonarRepository
{
    ProjetoSonar? ObterPorId(Guid projetoSonarId);

    bool ProjetoExiste(Guid projetoId);

    void Adicionar(ProjetoSonar projetoSonar);

    void Atualizar(ProjetoSonar projetoSonar);
}