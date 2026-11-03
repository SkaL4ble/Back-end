using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class Cargo
{
    public int CargoId { get; set; }

    public string NomeCargo { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
