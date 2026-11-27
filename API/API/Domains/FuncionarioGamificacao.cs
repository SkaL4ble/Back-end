using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class FuncionarioGamificacao
{
    public Guid UsuarioId { get; set; }

    public int ExperienciaTotal { get; set; }

    public int Nivel { get; set; }

    public DateTime DataAtualizacao { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
