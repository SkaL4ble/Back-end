using API.Applications.Conversoes;
using API.Domains;
using API.DTOs.CargoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class CargoService
    {
        private readonly ICargoRepository _repository;

        public CargoService(ICargoRepository repository)
        {
            _repository = repository;
        }

        private Cargo BuscarOuFalhar(int id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Cargo não encontrado");
        }

        private static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do cargo é obrigatório");

            return nome.Trim();
        }

        public List<ListarCargoDto> ListarCargo()
        {
            return _repository.ListarCargos()
                .Select(ConversaoCargo.ConverterParaDto)
                .ToList();
        }

        public ListarCargoDto ObterPorId(int id)
        {
            Cargo cargo = BuscarOuFalhar(id);
            return ConversaoCargo.ConverterParaDto(cargo);
        }

        public void AdicionarCargo(CriarCargoDto cargoDto)
        {
            cargoDto.NomeCargo = NormalizarNome(cargoDto.NomeCargo);

            if (_repository.NomeCargoExiste(cargoDto.NomeCargo))
                throw new DomainException("Cargo já existe");

            if(cargoDto.NomeCargo.Length > 30 || cargoDto.NomeCargo.Length < 3)
                throw new DomainException("O nome do cargo deve ter entre 3 e 30 caracteres");

            _repository.Adicionar(ConversaoCargo.ConverterParaDomain(cargoDto));
        }

        public void Atualizar(int id, CriarCargoDto cargoDto)
        {
            Cargo cargoBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(cargoDto.NomeCargo);

            bool nomeMudou = !string.Equals(
                cargoBanco.NomeCargo, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeCargoExiste(novoNome))
                throw new DomainException("Cargo já existe");

            cargoDto.NomeCargo = novoNome;
            _repository.Atualizar(id, ConversaoCargo.ConverterParaDomain(cargoDto));
        }

        public void Deletar(int id)
        {
            BuscarOuFalhar(id);
            _repository.Deletar(id);
        }
    }
}