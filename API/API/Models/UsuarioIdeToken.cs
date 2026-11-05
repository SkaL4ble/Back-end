using System;
using System.Collections.Generic;

namespace API.Models;

public partial class UsuarioIdeToken
{
    public Guid TokenId { get; set; }

    public Guid UsuarioId { get; set; }

    public string NomeDispositivo { get; set; } = null!;

    public string ApiKeyHash { get; set; } = null!;

    public DateTime? UltimoUso { get; set; }

    public bool Ativo { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
