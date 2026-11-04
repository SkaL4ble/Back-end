using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class TecnologiaRepository : ITecnologiaRepository
    {
        private readonly SkaL4bleContext _context;

        public TecnologiaRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<Tecnologia> ListarTecnologias()
        {
            return _context.Tecnologia
                .AsNoTracking()
                .OrderBy(t => t.NomeTecnologia)
                .ToList();
        }

        public Tecnologia? ObterPorId(int id)
        {
            return _context.Tecnologia.Find(id);
        }

        public bool NomeTecnologiaExiste(string nome)
        {
            return _context.Tecnologia.Any(t => t.NomeTecnologia == nome);
        }

        public bool PossuiVinculos(int id)
        {
            return _context.Tecnologia.Any(t =>
                t.TecnologiaId == id &&
                (t.Usuario.Any() ||
                 t.ConsolidadoMetricaTecnologia.Any() ||
                 t.HistoricoPontuacaoMetrica.Any()));
        }

        public void Adicionar(Tecnologia tecnologia)
        {
            _context.Tecnologia.Add(tecnologia);
            _context.SaveChanges();
        }

        public void Atualizar(int id, Tecnologia tecnologia)
        {
            Tecnologia? tecnologiaBanco = _context.Tecnologia.Find(id);

            if (tecnologiaBanco is null)
                return;

            tecnologiaBanco.NomeTecnologia = tecnologia.NomeTecnologia;
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            Tecnologia? tecnologiaBanco = _context.Tecnologia.Find(id);

            if (tecnologiaBanco is null)
                return;

            _context.Tecnologia.Remove(tecnologiaBanco);
            _context.SaveChanges();
        }
    }
}