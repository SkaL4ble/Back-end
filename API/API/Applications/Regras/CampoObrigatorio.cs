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

        // TIPO USUARIO
        public static void TipoUsuario(this string tipoUsuario) =>
            tipoUsuario.Validar("O tipo de usuário é obrigatório.");
    }
}