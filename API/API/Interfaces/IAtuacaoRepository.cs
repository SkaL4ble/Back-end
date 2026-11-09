using API.Domains;

namespace API.Interfaces
{
    public interface IAtuacaoRepository
    {
        List<Atuacao> ListarAtuacoes();
        Atuacao? ObterPorId(int id);
        bool NomeAtuacaoExiste(string nome);
        bool PossuiUsuarios(int id);
        void Adicionar(Atuacao atuacao);
        void Atualizar(int id, Atuacao atuacao);
        void Deletar(int id);
    }
}