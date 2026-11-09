using API.Domains;

namespace API.Interfaces
{
    public interface ITecnologiaRepository
    {
        List<Tecnologia> ListarTecnologias();
        Tecnologia? ObterPorId(int id);
        bool NomeTecnologiaExiste(string nome);
        bool PossuiVinculos(int id);
        void Adicionar(Tecnologia tecnologia);
        void Atualizar(int id, Tecnologia tecnologia);
        void Deletar(int id);
    }
}