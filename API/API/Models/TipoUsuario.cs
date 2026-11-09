using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TipoUsuario
{
    public Guid TipoUsuarioId { get; set; }

    public string NomeTipoUsuario { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
