using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TipoTicketUsuario
{
    public Guid UsuarioId { get; set; }

    public int TipoTicketId { get; set; }

    public decimal AfinidadeMedia { get; set; }

    public int ExperienciaTipoTicket { get; set; }

    public int NivelTipoTicket { get; set; }

    public int NumeroAtribuicoes { get; set; }

    public decimal ValorAtribuicoes { get; set; }

    public DateTime DataAtualizacao { get; set; }

    public virtual TipoTicket TipoTicket { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
