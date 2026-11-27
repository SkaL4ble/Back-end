namespace API.Applications.Services.Metrica;

public readonly record struct PeriodoMetrica(DateTime Inicio, DateTime Fim)
{
    public PeriodoMetrica Proximo =>
        Criar(Inicio.AddMonths(3));

    public static PeriodoMetrica Criar(DateTime data)
    {
        int primeiroMes = ((data.Month - 1) / 3) * 3 + 1;
        DateTime inicio = new(
            data.Year,
            primeiroMes,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc);

        return new PeriodoMetrica(
            inicio,
            inicio.AddMonths(3).AddTicks(-1));
    }
}
