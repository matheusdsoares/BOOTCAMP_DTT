namespace MinhaApi.Models
{
    public enum TipoMovimentacao
    {
        CriacaoLote = 0,
        AlteracaoStatus = 1,
        AlteracaoLocalizacao = 2,
        AlteracaoQualidade = 3,
        Embarque = 4,
        Recebimento = 5,
        Outro = 99
    }

    public class HistoricoMovimentacao
    {
        public int Id { get; set; }
        
        // Referência ao lote
        public int LoteId { get; set; }
        public string CodigoLote { get; set; } = "";
        
        // Tipo e descrição da movimentação
        public TipoMovimentacao TipoMovimentacao { get; set; }
        public string Descricao { get; set; } = "";
        
        // Dados da mudança
        public string? StatusAnterior { get; set; }
        public string? StatusNovo { get; set; }
        public string? LocalizacaoAnterior { get; set; }
        public string? LocalizacaoNova { get; set; }
        
        // Auditoria
        public DateTime DataHoraMovimentacao { get; set; }
        public string? UsuarioResponsavel { get; set; }
        public string? Observacoes { get; set; }
        
        // Dados adicionais (opcional - pode guardar JSON com mais info)
        public string? DadosAdicionais { get; set; }
    }
}