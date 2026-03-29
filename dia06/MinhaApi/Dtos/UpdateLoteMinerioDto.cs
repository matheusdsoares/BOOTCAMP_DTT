namespace MinhaApi.Dtos;

public class UpdateLoteMinerioDto
{
    public string CodigoLote { get; set; } = null!;
    public string MinaOrigem { get; set; } = null!;
    public decimal TeorFe { get; set; }
    public decimal Umidade { get; set; }
    public decimal? SiO2 { get; set; }
    public decimal? P { get; set; }
    public decimal Toneladas { get; set; }
    public DateTime DataProducao { get; set; }
    public int Status { get; set; }
    public string LocalizacaoAtual { get; set; } = null!;
}
