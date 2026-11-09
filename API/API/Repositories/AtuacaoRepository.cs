using API.Contexts;
using API.Domains;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class AtuacaoRepository : IAtuacaoRepository
    {
        private readonly SkaL4bleContext _context;

        public AtuacaoRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public List<Atuacao> ListarAtuacoes()
        {
            return _context.Atuacao
                .AsNoTracking()
                .OrderBy(a => a.NomeAtuacao)
                .ToList();
        }

        public Atuacao? ObterPorId(int id)
        {
            return _context.Atuacao.Find(id);
        }

        public bool NomeAtuacaoExiste(string nome)
        {
            return _context.Atuacao.Any(a => a.NomeAtuacao == nome);
        }

        public bool PossuiUsuarios(int id)
        {
            return _context.Atuacao
                .Where(a => a.AtuacaoId == id)
                .SelectMany(a => a.Usuario)
                .Any();
        }

        public void Adicionar(Atuacao atuacao)
        {
            _context.Atuacao.Add(atuacao);
            _context.SaveChanges();
        }

        public void Atualizar(int id, Atuacao atuacao)
        {
            Atuacao? atuacaoBanco = _context.Atuacao.Find(id);

            if (atuacaoBanco is null)
                return;

            atuacaoBanco.NomeAtuacao = atuacao.NomeAtuacao;
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            Atuacao? atuacaoBanco = _context.Atuacao.Find(id);

            if (atuacaoBanco is null)
                return;

            _context.Atuacao.Remove(atuacaoBanco);
            _context.SaveChanges();
        }
    }
}