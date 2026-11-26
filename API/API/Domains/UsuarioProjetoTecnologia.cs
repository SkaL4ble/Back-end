using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class UsuarioProjetoTecnologia
{
    public Guid ProjetoId { get; set; }

    public Guid UsuarioId { get; set; }

    public int TecnologiaId { get; set; }

    public long LinhasAdicionadas { get; set; }

    public decimal PontuacaoProjeto { get; set; }

    public DateTime DataUltimaParticipacao { get; set; }

    public virtual Projeto Projeto { get; set; } = null!;

    public virtual Tecnologia Tecnologia { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
