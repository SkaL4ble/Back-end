using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TrilhaTreinamentoTipoTicket
{
    public int TrilhaTreinamentoId { get; set; }

    public int TipoTicketId { get; set; }

    public int Ordem { get; set; }

    public bool Obrigatoria { get; set; }

    public virtual TipoTicket TipoTicket { get; set; } = null!;

    public virtual TrilhaTreinamento TrilhaTreinamento { get; set; } = null!;
}
