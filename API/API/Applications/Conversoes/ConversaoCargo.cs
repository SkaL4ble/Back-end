using API.Domains;
using API.DTOs.CargoDto;

namespace API.Applications.Conversoes
{
    public class ConversaoCargo
    {
        public static ListarCargoDto ConverterParaDto(Cargo cargo)
        {
            return new ListarCargoDto
            {
                CargoId = cargo.CargoId,
                NomeCargo = cargo.NomeCargo
            };
        }

        public static Cargo ConverterParaDomain(CriarCargoDto cargoDto)
        {
            return new Cargo
            {
                NomeCargo = cargoDto.NomeCargo
            };
        }
    }
}
