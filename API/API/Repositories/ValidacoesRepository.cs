using API.Contexts;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ValidacoesRepository : IValidacoesRepository
    {
        private readonly SkaL4bleContext _context;
        public bool UsuarioExiste(Guid? usuarioId)
        {
            return usuarioId.HasValue && _context.Usuario.Find(usuarioId.Value) != null;
        }

        public bool StatusProjetoExiste(int? statusId)
        {
            return statusId.HasValue && _context.StatusProjeto.Find(statusId.Value) != null;
        }

        public bool TipoProjetoExiste(int? tipoId)
        {
            return tipoId.HasValue && _context.TipoProjeto.Find(tipoId.Value) != null;
        }

        public bool AtuacaoExiste(int id)
        {
            return _context.Atuacao.Find(id) != null;
        }

        public bool CargoExiste(int id)
        {
            return _context.Cargo.Find(id) != null;
        }

        public bool TipoUsuarioExiste(Guid id)
        {
            return _context.TipoUsuario.Find(id) != null;
        }

        public bool TecnologiaExiste(int id)
        {
            return _context.Tecnologia.Find(id) != null;
        }

        public bool ProjetosExiste(Guid id)
        {
            return _context.Projeto.Find(id) != null;
        }

        public bool TicketExiste(int id)
        {
            return _context.Ticket.Find(id) != null;
        }

        public bool TipoTicketExiste(int tipoTicketId)
        {
            return _context.TipoTicket.Find(tipoTicketId) != null;
        }

        public bool StatusTicketExiste(int statusTicketId)
        {
            return _context.StatusTicket.Find(statusTicketId) != null;
        }
    }
}
