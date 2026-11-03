using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class StatusTicket
{
    public int StatusTicketId { get; set; }

    public string NomeStatusTicket { get; set; } = null!;

    public virtual ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();
}
