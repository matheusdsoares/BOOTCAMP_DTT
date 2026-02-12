namespace MinhaApi.Queue
{
    public record ProcessarLoteMessage(
        int LoteId,
        string CodigoLote,
        decimal TeorFe,
        decimal Umidade,
        DateTime DataProducaoUtc,
        string Acao 
    );
}