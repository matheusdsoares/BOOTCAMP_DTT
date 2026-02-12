namespace MinhaApi.Models
{
    public enum StatusLote
    {
        EmEstoque = 0,
        EmTransporte = 1,
        Embarcado = 2
    }

    public enum ClassificacaoQualidade
    {
        Baixa = 0,
        Padrao = 1,
        Premium = 2
    }

    public class LoteMinerio
    {
        public int Id { get; set; }
        
        // Identificação e rastreio
        public string CodigoLote { get; set; } = "";
        public string MinaOrigem { get; set; } = "";
        
        // Qualidade (simplificada)
        public decimal TeorFe { get; set; }
        public decimal Umidade { get; set; }
        public decimal? SiO2 { get; set; }
        public decimal? P { get; set; }
        
        // Logística básica
        public decimal Toneladas { get; set; }
        public DateTime DataProducao { get; set; }
        public StatusLote Status { get; set; }
        public string LocalizacaoAtual { get; set; } = "";
    }
}