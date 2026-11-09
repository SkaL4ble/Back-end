using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TipoTicket
{
    public int TipoTicketId { get; set; }

    public string NomeTipoTicket { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
