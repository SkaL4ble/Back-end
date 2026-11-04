using API.Applications.Conversoes;
using API.Domains;
using API.DTOs.TecnologiaDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class TecnologiaService
    {
        private readonly ITecnologiaRepository _repository;

        public TecnologiaService(ITecnologiaRepository repository)
        {
            _repository = repository;
        }

        private Tecnologia BuscarOuFalhar(int id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Tecnologia não encontrada");
        }

        private static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome da tecnologia é obrigatório");

            return nome.Trim();
        }

        public List<ListarTecnologiaDto> ListarTecnologias()
        {
            return _repository.ListarTecnologias()
                .Select(ConversaoTecnologia.ConverterParaDto)
                .ToList();
        }

        public ListarTecnologiaDto ObterPorId(int id)
        {
            Tecnologia tecnologia = BuscarOuFalhar(id);
            return ConversaoTecnologia.ConverterParaDto(tecnologia);
        }

        public void AdicionarTecnologia(CriarTecnologiaDto tecnologiaDto)
        {
            tecnologiaDto.NomeTecnologia = NormalizarNome(tecnologiaDto.NomeTecnologia);

            if (_repository.NomeTecnologiaExiste(tecnologiaDto.NomeTecnologia))
                throw new DomainException("Tecnologia já existe");

            _repository.Adicionar(ConversaoTecnologia.ConverterParaDomain(tecnologiaDto));
        }

        public void Atualizar(int id, CriarTecnologiaDto tecnologiaDto)
        {
            Tecnologia tecnologiaBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(tecnologiaDto.NomeTecnologia);

            bool nomeMudou = !string.Equals(
                tecnologiaBanco.NomeTecnologia, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeTecnologiaExiste(novoNome))
                throw new DomainException("Tecnologia já existe");

            tecnologiaDto.NomeTecnologia = novoNome;
            _repository.Atualizar(id, ConversaoTecnologia.ConverterParaDomain(tecnologiaDto));
        }

        public void Deletar(int id)
        {
            BuscarOuFalhar(id);

            if (_repository.PossuiVinculos(id))
                throw new DomainException("Não é possível excluir: há usuários, métricas ou histórico vinculados a esta tecnologia");

            _repository.Deletar(id);
        }
    }
}