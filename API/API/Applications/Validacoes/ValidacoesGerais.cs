using API.Domains;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Validacoes
{
    public class ValidacoesGerais
    {
        private readonly IValidacoesRepository _repository;

        public ValidacoesGerais(IValidacoesRepository repository)
        {
            _repository = repository;
        }
        public string NormalizarNome(string? nome, int? minimo = 3, int? maximo = 50)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome é obrigatório");
            if (nome.Length > maximo || nome.Length < minimo)
                throw new DomainException("Tamanho de caracteres invalido");

            return nome.Trim();
        }

        public void ValidarUsuario(Guid? usuarioId)
        {
            if (!_repository.UsuarioExiste(usuarioId))
                throw new DomainException("Usuário não encontrado ou inativo");
        }

        public void ValidarProjeto(Guid? projetoId)
        {
            if (!_repository.ProjetosExiste(projetoId))
                throw new DomainException("Projeto não encontrado ou inativo");
        }

        public void ValidarTipoTicket(int? tipoTicketId)
        {
            if (!_repository.TipoTicketExiste(tipoTicketId))
                throw new DomainException("Tipo de ticket não encontrado");
        }

        public void ValidarStatusTicket(int? statusTicketId)
        {
            if (!_repository.StatusTicketExiste(statusTicketId))
                throw new DomainException("Status de ticket não encontrado");
        }

        public void ValidarCargo(int? cargoId)
        {
            if (!_repository.CargoExiste(cargoId))
                throw new DomainException("Cargo não encontrado");
        }

        public void ValidarTipoUsuario(Guid? tipoUsuarioId)
        {
            if (!_repository.TipoUsuarioExiste(tipoUsuarioId))
                throw new DomainException("Tipo de usuário não encontrado");
        }

        public void ValidarTecnologia(int? tecnologiaId)
        {
            if (!_repository.TecnologiaExiste(tecnologiaId))
                throw new DomainException("Tecnologia não encontrada");
        }
        public void ValidarTicket(int? ticketId)
        {
            if (!_repository.TicketExiste(ticketId))
                throw new DomainException("Tecnologia não encontrada");
        }

        public void ValidarAtuacao(int? atuacaoId)
        {
            if (!_repository.AtuacaoExiste(atuacaoId))
                throw new DomainException(" não encontrada");
        }
        public void ValidarStatusProjeto(int? statusProjetoId)
        {
            if (!_repository.StatusProjetoExiste(statusProjetoId))
                throw new DomainException(" não encontrada");
        }
        public void ValidarTipoProjeto(int? projetoId)
        {
            if (!_repository.TipoProjetoExiste(projetoId))
                throw new DomainException(" não encontrada");
        }
    }
}
