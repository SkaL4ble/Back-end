using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TipoTicket
{
    public int TipoTicketId { get; set; }

    public string NomeTipoTicket { get; set; } = null!;

    public virtual ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();

    public virtual ICollection<TipoTicketUsuario> TipoTicketUsuario { get; set; } = new List<TipoTicketUsuario>();

    public virtual ICollection<TrilhaTreinamentoTipoTicket> TrilhaTreinamentoTipoTicket { get; set; } = new List<TrilhaTreinamentoTipoTicket>();
}
