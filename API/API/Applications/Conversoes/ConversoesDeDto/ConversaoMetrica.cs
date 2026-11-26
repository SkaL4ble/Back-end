using API.Domains;
using API.DTOs.MetricaDto;

namespace API.Applications.Conversoes.ConversoesDeDto
{
    public class ConversaoMetrica
    {
        public static FuncionarioMetricaHistorico ConverterParaHistorico(FuncionarioMetricaAtual metrica)
        {
            return new FuncionarioMetricaHistorico
            {
                UsuarioId = metrica.UsuarioId,

                PeriodoInicio = metrica.PeriodoInicio,
                PeriodoFim = metrica.PeriodoFim,

                NotaQuantidade = metrica.NotaQuantidade,
                NotaVelocidade = metrica.NotaVelocidade,
                NotaQualidade = metrica.QualidadeSonar,

                TicketsAtribuidos =
                    metrica.TicketsAtribuidos,

                TicketsConcluidos =
                    metrica.TicketsConcluidos,

                TicketsAltaConcluidos =
                    metrica.TicketsAltaConcluidos,

                TicketsMediaConcluidos =
                    metrica.TicketsMediaConcluidos,

                TicketsBaixaConcluidos =
                    metrica.TicketsBaixaConcluidos,

                PontosQuantidade =
                    metrica.PontosQuantidade,

                TicketsComPrazo =
                    metrica.TicketsComPrazo,

                TicketsConcluidosNoPrazo =
                    metrica.TicketsConcluidosNoPrazo,

                TicketsConcluidosAtrasados =
                    metrica.TicketsConcluidosAtrasados,

                TicketsAbertosAtrasados =
                    metrica.TicketsAbertosAtrasados,

                DiasAtrasoAcumulados =
                    metrica.DiasAtrasoAcumulados,

                DiasSemAtividade =
                    metrica.DiasSemAtividade,

                QualidadeSonar =
                    metrica.QualidadeSonar,

                QualidadeManutencao =
                    metrica.QualidadeManutencao,

                QualidadeConfiabilidade =
                    metrica.QualidadeConfiabilidade,

                QualidadeSeguranca =
                    metrica.QualidadeSeguranca,

                IssuesManutencao =
                    metrica.IssuesManutencao,

                IssuesConfiabilidade =
                    metrica.IssuesConfiabilidade,

                IssuesSeguranca =
                    metrica.IssuesSeguranca,

                DataQualidadeSonar =
                    metrica.DataUltimaQualidadeSonar,

                DataFechamento = DateTime.UtcNow
            };
        }
    }
}
