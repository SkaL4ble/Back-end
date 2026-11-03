using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class Atuacao
{
    public int AtuacaoId { get; set; }

    public string NomeAtuacao { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
