using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class ProjetoSonar
{
    public Guid ProjetoSonarId { get; set; }

    public Guid ProjetoId { get; set; }

    public string ChaveSonar { get; set; } = null!;

    public string NomeSonar { get; set; } = null!;

    public string? Descricao { get; set; }

    public string Origem { get; set; } = null!;

    public string TokenSonarCriptografado { get; set; } = null!;

    public DateTime DataCriacao { get; set; }

    public int WorkflowId { get; set; }

    public virtual Projeto Projeto { get; set; } = null!;
}
