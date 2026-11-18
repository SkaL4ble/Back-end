using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TokenRedefinicaoSenha
{
    public int TokenRedefinicaoSenhaId { get; set; }

    public Guid UsuarioId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiraEm { get; set; }

    public bool Utilizado { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
