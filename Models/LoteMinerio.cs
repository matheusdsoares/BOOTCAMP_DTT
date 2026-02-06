using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaApi.Models

{
    public enum StatusLote
    {
        EmEstoque = 0,
        EmTransporte = 1,
        Embarcado = 2
    }

   [Table("lotes_minerio")]
    public class LoteMinerio
{
    public int Id { get; set; }

    [Column("codigo_lote")]
    public string CodigoLote { get; set; } = null!;

    [Column("mina_origem")]
    public string MinaOrigem { get; set; } = null!;

    [Column("localizacao_atual")]
    public string LocalizacaoAtual { get; set; } = null!;

    [Column("teor_fe")]
    public decimal TeorFe { get; set; }

    [Column("umidade")]
    public decimal Umidade { get; set; }

    [Column("sio2")]
    public decimal? SiO2 { get; set; }

    [Column("p")]
    public decimal? P { get; set; }

    [Column("toneladas")]
    public decimal Toneladas { get; set; }

    [Column("data_producao")]
    public DateTime DataProducao { get; set; }

    [Column("status")]
    public StatusLote Status { get; set; }
}
}