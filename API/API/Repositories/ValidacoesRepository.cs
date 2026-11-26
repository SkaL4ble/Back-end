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

<<<<<<< HEAD
        public bool AtuacaoExiste(int? id)
=======
        public bool AtuacaoExiste(int id)
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        {
            return _context.Atuacao.Find(id) != null;
        }

<<<<<<< HEAD
        public bool CargoExiste(int? id)
=======
        public bool CargoExiste(int id)
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        {
            return _context.Cargo.Find(id) != null;
        }

<<<<<<< HEAD
        public bool TipoUsuarioExiste(Guid? id)
=======
        public bool TipoUsuarioExiste(Guid id)
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        {
            return _context.TipoUsuario.Find(id) != null;
        }

<<<<<<< HEAD
        public bool TecnologiaExiste(int? id)
=======
        public bool TecnologiaExiste(int id)
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        {
            return _context.Tecnologia.Find(id) != null;
        }

<<<<<<< HEAD
        public bool ProjetosExiste(Guid? id)
=======
        public bool ProjetosExiste(Guid id)
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        {
            return _context.Projeto.Find(id) != null;
        }

<<<<<<< HEAD
        public bool TicketExiste(int? id)
=======
        public bool TicketExiste(int id)
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        {
            return _context.Ticket.Find(id) != null;
        }

<<<<<<< HEAD
        public bool TipoTicketExiste(int? tipoTicketId)
=======
        public bool TipoTicketExiste(int tipoTicketId)
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        {
            return _context.TipoTicket.Find(tipoTicketId) != null;
        }

<<<<<<< HEAD
        public bool StatusTicketExiste(int? statusTicketId)
=======
        public bool StatusTicketExiste(int statusTicketId)
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        {
            return _context.StatusTicket.Find(statusTicketId) != null;
        }
    }
}
