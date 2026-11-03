using API.Exceptions;

namespace API.Applications.Regras
{
    public static class CampoObrigatorio
    {
        // O THIS possibilita o POLIMORFISMO DE EXTENSÃO
        private static void Validar(this string nome, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new DomainException(mensagem);
            }
        }

        // NOME GENÉRICO
        public static void Nome(this string nome) =>
            nome.Validar("O nome é obrigatório.");

        // EXEMPLO
        public static void Exemplo(this string exemplo) =>
            exemplo.Validar("O exemplo é obrigatório.");
    }
}