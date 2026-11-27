using API.Domains;
using API.DTOs.GameficacaoDto;

namespace API.Interfaces;

public interface IGameficacaoRepository
{
    // ================================ CONSULTAS ================================

    UsuarioGameficacao? ObterPorUsuarioId(Guid usuarioId);

    List<TipoTicketUsuario>ListarTiposTicketPorUsuarioId(Guid usuarioId);

    TipoTicketUsuario? ObterTipoTicketDoUsuario(Guid usuarioId, int tipoTicketId);

    List<DemandaTipoTicketDto> ListarDemandasPorTipoTicket();

    List<TrilhaTreinamento> ListarTrilhasPorTipoTicket(int tipoTicketId);

    // ================================ CADASTRO ================================

    void AdicionarGameficacao(UsuarioGameficacao gameficacao);

    void AdicionarTipoTicketUsuario(TipoTicketUsuario tipoTicketUsuario);

    // ================================ ATUALIZAÇÃO ================================

    void AtualizarGameficacao(UsuarioGameficacao gameficacao);

    void AtualizarTipoTicketUsuario(TipoTicketUsuario tipoTicketUsuario);

    // ================================ PERSISTÊNCIA ================================

    void Salvar();
}