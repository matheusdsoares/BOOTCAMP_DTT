namespace MinhaApi.Services
{
    using MinhaApi.Models;

    public static class CalculadorPenalidades
    {
        // Limites de umidade aceitáveis
        private const decimal UMIDADE_IDEAL = 8.0m;        // Até 8% sem penalidade
        private const decimal UMIDADE_TOLERAVEL = 10.0m;   // 8-10% penalidade leve
        private const decimal UMIDADE_ALTA = 12.0m;        // 10-12% penalidade moderada
        // Acima de 12% = penalidade severa

        // Taxas de penalidade (% de desconto sobre o valor)
        private const decimal TAXA_LEVE = 2.0m;       // 2% de desconto
        private const decimal TAXA_MODERADA = 5.0m;   // 5% de desconto
        private const decimal TAXA_SEVERA = 10.0m;    // 10% de desconto
        private const decimal TAXA_CRITICA = 15.0m;   // 15% de desconto

        /// <summary>
        /// Calcula a penalidade financeira baseada na umidade do lote
        /// </summary>
        public static ResultadoPenalidade CalcularPenalidade(LoteMinerio lote)
        {
            var classificacaoUmidade = ClassificarUmidade(lote.Umidade);
            var percentualPenalidade = ObterPercentualPenalidade(lote.Umidade);
            var valorOriginal = CalculadorPreco.CalcularValorTotal(lote);
            var valorPenalidade = valorOriginal * (percentualPenalidade / 100);
            var valorFinal = valorOriginal - valorPenalidade;
            var excessoUmidade = lote.Umidade > UMIDADE_IDEAL 
                ? lote.Umidade - UMIDADE_IDEAL 
                : 0;

            return new ResultadoPenalidade
            {
                CodigoLote = lote.CodigoLote,
                UmidadeAtual = lote.Umidade,
                UmidadeIdeal = UMIDADE_IDEAL,
                ExcessoUmidade = excessoUmidade,
                ClassificacaoUmidade = classificacaoUmidade,
                PercentualPenalidade = percentualPenalidade,
                ValorOriginal = valorOriginal,
                ValorPenalidade = valorPenalidade,
                ValorFinal = valorFinal,
                PerdaFinanceira = valorPenalidade,
                TemPenalidade = percentualPenalidade > 0,
                Recomendacao = GerarRecomendacao(lote.Umidade, excessoUmidade)
            };
        }

        /// <summary>
        /// Classifica o nível de umidade do lote
        /// </summary>
        public static string ClassificarUmidade(decimal umidade)
        {
            if (umidade <= UMIDADE_IDEAL)
                return "Ideal";
            else if (umidade <= UMIDADE_TOLERAVEL)
                return "Tolerável";
            else if (umidade <= UMIDADE_ALTA)
                return "Alta";
            else
                return "Crítica";
        }

        /// <summary>
        /// Retorna o percentual de penalidade baseado na umidade
        /// </summary>
        public static decimal ObterPercentualPenalidade(decimal umidade)
        {
            if (umidade <= UMIDADE_IDEAL)
                return 0m; // Sem penalidade
            else if (umidade <= UMIDADE_TOLERAVEL)
                return TAXA_LEVE; // 2%
            else if (umidade <= UMIDADE_ALTA)
                return TAXA_MODERADA; // 5%
            else if (umidade <= 15.0m)
                return TAXA_SEVERA; // 10%
            else
                return TAXA_CRITICA; // 15%
        }

        /// <summary>
        /// Calcula quanto seria economizado se a umidade fosse reduzida
        /// </summary>
        public static decimal CalcularEconomiaPotencial(LoteMinerio lote, decimal umidadeAlvo)
        {
            if (umidadeAlvo >= lote.Umidade)
                return 0m; // Não há economia se a umidade alvo for maior ou igual

            var penalAtual = CalcularPenalidade(lote);
            
            // Simular lote com umidade reduzida
            var loteSimulado = new LoteMinerio
            {
                CodigoLote = lote.CodigoLote,
                TeorFe = lote.TeorFe,
                Umidade = umidadeAlvo,
                SiO2 = lote.SiO2,
                Toneladas = lote.Toneladas
            };

            var penalComMelhoria = CalcularPenalidade(loteSimulado);
            
            return penalAtual.PerdaFinanceira - penalComMelhoria.PerdaFinanceira;
        }

        /// <summary>
        /// Calcula o custo por ponto percentual de umidade excedente
        /// </summary>
        public static decimal CalcularCustoPorPontoPercentual(LoteMinerio lote)
        {
            var penalidade = CalcularPenalidade(lote);
            
            if (penalidade.ExcessoUmidade == 0)
                return 0m;

            return penalidade.PerdaFinanceira / penalidade.ExcessoUmidade;
        }

        /// <summary>
        /// Retorna a tabela de penalidades configurada
        /// </summary>
        public static object ObterTabelaPenalidades()
        {
            return new
            {
                Faixas = new[]
                {
                    new
                    {
                        Faixa = $"0% - {UMIDADE_IDEAL}%",
                        Classificacao = "Ideal",
                        Penalidade = "0%",
                        Descricao = "Sem penalidade"
                    },
                    new
                    {
                        Faixa = $"{UMIDADE_IDEAL}% - {UMIDADE_TOLERAVEL}%",
                        Classificacao = "Tolerável",
                        Penalidade = $"{TAXA_LEVE}%",
                        Descricao = "Penalidade leve"
                    },
                    new
                    {
                        Faixa = $"{UMIDADE_TOLERAVEL}% - {UMIDADE_ALTA}%",
                        Classificacao = "Alta",
                        Penalidade = $"{TAXA_MODERADA}%",
                        Descricao = "Penalidade moderada"
                    },
                    new
                    {
                        Faixa = $"{UMIDADE_ALTA}% - 15%",
                        Classificacao = "Severa",
                        Penalidade = $"{TAXA_SEVERA}%",
                        Descricao = "Penalidade severa"
                    },
                    new
                    {
                        Faixa = "Acima de 15%",
                        Classificacao = "Crítica",
                        Penalidade = $"{TAXA_CRITICA}%",
                        Descricao = "Penalidade crítica"
                    }
                },
                Observacao = "Penalidades são aplicadas sobre o valor total do lote"
            };
        }

        /// <summary>
        /// Calcula impacto da umidade em múltiplos lotes
        /// </summary>
        public static RelatorioImpactoUmidade CalcularImpactoGeral(List<LoteMinerio> lotes)
        {
            var totalLotes = lotes.Count;
            var lotesComPenalidade = 0;
            var totalPerdas = 0m;
            var totalValorOriginal = 0m;

            var detalhesPorLote = lotes.Select(lote =>
            {
                var penalidade = CalcularPenalidade(lote);
                totalValorOriginal += penalidade.ValorOriginal;
                totalPerdas += penalidade.PerdaFinanceira;
                
                if (penalidade.TemPenalidade)
                    lotesComPenalidade++;

                return new
                {
                    lote.CodigoLote,
                    lote.Umidade,
                    penalidade.ClassificacaoUmidade,
                    penalidade.PercentualPenalidade,
                    penalidade.PerdaFinanceira
                };
            }).ToList();

            return new RelatorioImpactoUmidade
            {
                TotalLotes = totalLotes,
                LotesComPenalidade = lotesComPenalidade,
                LotesSemPenalidade = totalLotes - lotesComPenalidade,
                PercentualLotesAfetados = totalLotes > 0 
                    ? Math.Round((decimal)lotesComPenalidade / totalLotes * 100, 2) 
                    : 0,
                ValorTotalOriginal = totalValorOriginal,
                TotalPerdas = totalPerdas,
                PercentualPerda = totalValorOriginal > 0 
                    ? Math.Round(totalPerdas / totalValorOriginal * 100, 2) 
                    : 0,
                UmidadeMedia = lotes.Any() ? lotes.Average(l => l.Umidade) : 0,
                DetalhesPorLote = detalhesPorLote
            };
        }

        // Método auxiliar privado
        private static string GerarRecomendacao(decimal umidade, decimal excesso)
        {
            if (excesso == 0)
                return "Umidade dentro do padrão ideal. Nenhuma ação necessária.";
            
            if (umidade <= UMIDADE_TOLERAVEL)
                return $"Umidade levemente elevada (+{excesso:F2}%). Considere secagem leve para evitar penalidades.";
            
            if (umidade <= UMIDADE_ALTA)
                return $"Umidade alta (+{excesso:F2}%). Recomenda-se processo de secagem antes do embarque.";
            
            return $"Umidade crítica (+{excesso:F2}%). URGENTE: Secagem obrigatória antes do embarque para evitar grandes perdas financeiras.";
        }
    }

    // Classes auxiliares
    public class ResultadoPenalidade
    {
        public string CodigoLote { get; set; } = "";
        public decimal UmidadeAtual { get; set; }
        public decimal UmidadeIdeal { get; set; }
        public decimal ExcessoUmidade { get; set; }
        public string ClassificacaoUmidade { get; set; } = "";
        public decimal PercentualPenalidade { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorPenalidade { get; set; }
        public decimal ValorFinal { get; set; }
        public decimal PerdaFinanceira { get; set; }
        public bool TemPenalidade { get; set; }
        public string Recomendacao { get; set; } = "";
    }

    public class RelatorioImpactoUmidade
    {
        public int TotalLotes { get; set; }
        public int LotesComPenalidade { get; set; }
        public int LotesSemPenalidade { get; set; }
        public decimal PercentualLotesAfetados { get; set; }
        public decimal ValorTotalOriginal { get; set; }
        public decimal TotalPerdas { get; set; }
        public decimal PercentualPerda { get; set; }
        public decimal UmidadeMedia { get; set; }
        public object DetalhesPorLote { get; set; } = new { };
    }
}