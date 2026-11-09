using System;
using System.Collections.Generic;

namespace API.Models;

public partial class StatusTicket
{
    public int StatusTicketId { get; set; }

    public string NomeStatusTicket { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
