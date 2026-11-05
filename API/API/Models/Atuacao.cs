using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Atuacao
{
    public int AtuacaoId { get; set; }

    public string NomeAtuacao { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
