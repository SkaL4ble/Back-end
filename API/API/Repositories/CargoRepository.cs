using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        private readonly SkaL4bleContext _context;

        public CargoRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<Cargo> ListarCargos()
        {
            return _context.Cargo.AsNoTracking().OrderByDescending(c => c.NomeCargo).ToList();
        }

        public Cargo ObterPorId(int id)
        {
            return _context.Cargo.Find(id);
        }

        public bool NomeCargoExiste(string nome)
        {
            return _context.Cargo.Any(c => c.NomeCargo == nome);
        }

        public void Adicionar(Cargo cargo)
        {
            _context.Cargo.Add(cargo);
            _context.SaveChanges();
        }

        public void Deletar (int id)
        {
            Cargo cargoBanco = _context.Cargo.Find(id);

                if(cargoBanco == null)
            {
                return;
            }
            _context.Cargo.Remove(cargoBanco);
            _context.SaveChanges();
        }

        public void Atualizar(int id, Cargo cargo)
        {
            Cargo cargoBanco = _context.Cargo.Find(id);

            if(cargoBanco == null)
            {
                return;
            }

            cargoBanco.NomeCargo = cargo.NomeCargo;

            _context.SaveChanges();
        }
    }
}
