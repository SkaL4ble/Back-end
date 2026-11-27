using API.Domains;
using API.DTOs;

namespace API.Interfaces;

public interface IGameficacaoRepository
{
    UsuarioGameficacao? ObterPorUsuarioId(Guid usuarioId);

    List<TecnologiaUsuario> ListarTecnologiasPorUsuarioId(Guid usuarioId);

    TecnologiaUsuario? ObterTecnologiaDoUsuario(Guid usuarioId, int tecnologiaId);

    List<DemandaTecnologiaDto> ListarDemandasTecnologicas();

    List<TrilhaTreinamento> ListarTrilhasPorTecnologia(int tecnologiaId);

    void AdicionarGameficacao(UsuarioGameficacao gameficacao);

    void AtualizarGameficacao(UsuarioGameficacao gameficacao);

    void AdicionarTecnologiaUsuario( TecnologiaUsuario tecnologiaUsuario);

    void AtualizarTecnologiaUsuario(TecnologiaUsuario tecnologiaUsuario);

    void Salvar();
}