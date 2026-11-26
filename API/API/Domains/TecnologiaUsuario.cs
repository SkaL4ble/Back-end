using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TecnologiaUsuario
{
    public int TecnologiaId { get; set; }

    public Guid UsuarioId { get; set; }

    public decimal AfinidadeMedia { get; set; }

    public int NumeroAtribuicoes { get; set; }

    public decimal ValorAtribuicoes { get; set; }

    public DateTime DataUltimaAtualizacao { get; set; }

    public virtual Tecnologia Tecnologia { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
