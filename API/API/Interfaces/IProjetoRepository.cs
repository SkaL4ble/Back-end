using API.Domains;

namespace API.Interfaces
{
    public interface IProjetoRepository
    {
        Projeto? ObterPorId(Guid id);
    }
}
