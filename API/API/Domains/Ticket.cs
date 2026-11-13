using System;
using System.Collections.Generic;

namespace API.Domains;

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

    public virtual ICollection<TicketUsuario> TicketUsuario { get; set; } = new List<TicketUsuario>();

    public virtual TipoTicket TipoTicket { get; set; } = null!;

    public virtual Usuario? UsuarioResponsavelNavigation { get; set; }
<<<<<<< HEAD
=======

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
>>>>>>> fce64ec2d6d75cf75b8d1c0bcf73ce76e6e43367
}
