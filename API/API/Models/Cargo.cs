using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Cargo
{
    public int CargoId { get; set; }

    public string NomeCargo { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
