namespace API.Interfaces
{
    public interface IValidacoesRepository
    {
        bool UsuarioExiste(Guid? usuarioId);
        bool StatusProjetoExiste(int? statusId);
        bool TipoProjetoExiste(int? tipoId);
        bool AtuacaoExiste(int id);
        bool CargoExiste(int id);
        bool TipoUsuarioExiste(Guid id);
        bool TecnologiaExiste(int id);
        bool ProjetosExiste(Guid id);
        bool TicketExiste(int id);
        bool TipoTicketExiste(int tipoTicketId);
        bool StatusTicketExiste(int statusTicketId);
    }
}
