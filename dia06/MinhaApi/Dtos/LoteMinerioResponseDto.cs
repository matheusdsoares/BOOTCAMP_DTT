using MinhaApi.Models;

namespace MinhaApi.Dtos
{
    public record LoteMinerioResponseDto(
        int Id,
        string CodigoLote,
        string MinaOrigem,
        decimal TeorFe,
        decimal Umidade,
        decimal? SiO2,
        decimal? P,
        decimal Toneladas,
        DateTime DataProducao,      
        StatusLote Status,
        string LocalizacaoAtual
    );
}