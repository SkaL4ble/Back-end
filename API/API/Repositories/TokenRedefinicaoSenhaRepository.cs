using Microsoft.EntityFrameworkCore;
using API.Contexts;
using API.Domains;
using API.Interfaces;

namespace API.Repositories
{
    public class TokenRedefinicaoSenhaRepository : ITokenRedefinicaoSenhaRepository
    {
        private readonly SkaL4bleContext _context;

        public TokenRedefinicaoSenhaRepository(SkaL4bleContext context)
        {
            _context = context;
        }

        public async Task CadastrarAsync(TokenRedefinicaoSenha token)
        {
            _context.TokenRedefinicaoSenha.Add(token);

            await _context.SaveChangesAsync();
        }

        public async Task<TokenRedefinicaoSenha?> BuscarPorHashAsync(string tokenHash)
        {
            return await _context.TokenRedefinicaoSenha.FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.Utilizado);
        }

        public async Task AtualizarAsync(TokenRedefinicaoSenha token)
        {
            _context.TokenRedefinicaoSenha.Update(token);

            await _context.SaveChangesAsync();
        }
    }
}