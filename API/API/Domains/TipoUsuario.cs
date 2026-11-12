using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TipoUsuario
{
    public Guid TipoUsuarioId { get; set; }

    public string CodigoTipoUsuario { get; set; } = null!;

    public string NomeTipoUsuario { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
