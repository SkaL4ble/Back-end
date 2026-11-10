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

    public string? Plataforma { get; set; }

    public string TokenSonarCriptografado { get; set; } = null!;

    public string? TokenGithubCriptografado { get; set; }

    public DateTime DataCriacao { get; set; }

    public virtual Projeto Projeto { get; set; } = null!;
}
