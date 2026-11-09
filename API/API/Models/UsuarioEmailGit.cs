using System;
using System.Collections.Generic;

namespace API.Models;

public partial class UsuarioEmailGit
{
    public int UsuarioEmailGitId { get; set; }

    public Guid UsuarioId { get; set; }

    public string EmailGit { get; set; } = null!;

    public bool Ativo { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
