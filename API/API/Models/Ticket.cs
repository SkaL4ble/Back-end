using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public string NomeTicket { get; set; } = null!;

    public int Prioridade { get; set; }

    public DateTime DataCriacao { get; set; }

    public DateTime? DataPrazo { get; set; }

    public DateTime? DataEntrega { get; set; }

    public bool Ativo { get; set; }

    public Guid? UsuarioResponsavel { get; set; }

    public Guid ProjetoId { get; set; }

    public int TipoTicketId { get; set; }

    public int StatusTicketId { get; set; }

    public virtual Projeto Projeto { get; set; } = null!;

    public virtual StatusTicket StatusTicket { get; set; } = null!;

    public virtual TipoTicket TipoTicket { get; set; } = null!;

    public virtual Usuario? UsuarioResponsavelNavigation { get; set; }

    public virtual ICollection<Usuario> Colaboradors { get; set; } = new List<Usuario>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
