using API.Contexts;
using API.Domains;

namespace API.Interfaces
{
    public interface ICargoRepository
    {
        public List<Cargo> ListarCargos();
        public Cargo ObterPorId(int id);
        public bool NomeCargoExiste(string nome);
        public void Adicionar(Cargo cargo);
        public void Deletar(int id);
        public void Atualizar(int id, Cargo cargo);
    }
}
