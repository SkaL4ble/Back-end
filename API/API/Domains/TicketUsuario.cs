using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TicketUsuario
{
    public int TicketId { get; set; }

    public Guid UsuarioId { get; set; }

    public bool Autor { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
