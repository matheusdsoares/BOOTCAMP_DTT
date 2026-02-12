namespace MinhaApi.Services
{
    using MinhaApi.Models;

    public static class CalculadorPreco
    {
        // Preços base por tonelada (em USD) - Ajuste conforme necessário
        private const decimal PRECO_PREMIUM = 120.00m;
        private const decimal PRECO_PADRAO = 85.00m;
        private const decimal PRECO_BAIXA = 55.00m;

        /// <summary>
        /// Calcula o preço por tonelada baseado na classificação do minério
        /// </summary>
        /// <param name="lote">Lote de minério</param>
        /// <returns>Preço por tonelada em USD</returns>
        public static decimal CalcularPrecoPorTonelada(LoteMinerio lote)
        {
            var classificacao = ClassificadorQualidade.ObterClassificacao(lote);

            return classificacao switch
            {
                "Premium" => PRECO_PREMIUM,
                "Baixa" => PRECO_BAIXA,
                _ => PRECO_PADRAO
            };
        }

        /// <summary>
        /// Calcula o valor total do lote (preço por tonelada * toneladas)
        /// </summary>
        /// <param name="lote">Lote de minério</param>
        /// <returns>Valor total em USD</returns>
        public static decimal CalcularValorTotal(LoteMinerio lote)
        {
            var precoPorTonelada = CalcularPrecoPorTonelada(lote);
            return precoPorTonelada * lote.Toneladas;
        }

        /// <summary>
        /// Retorna informações completas de precificação do lote
        /// </summary>
        public static object ObterDetalhamentoPrecificacao(LoteMinerio lote)
        {
            var classificacao = ClassificadorQualidade.ObterClassificacao(lote);
            var precoPorTonelada = CalcularPrecoPorTonelada(lote);
            var valorTotal = CalcularValorTotal(lote);

            return new
            {
                CodigoLote = lote.CodigoLote,
                Toneladas = lote.Toneladas,
                Classificacao = classificacao,
                PrecoPorTonelada = precoPorTonelada,
                ValorTotal = valorTotal,
                Moeda = "USD"
            };
        }

        /// <summary>
        /// Calcula a diferença de valor se o lote fosse de qualidade Premium
        /// </summary>
        public static decimal CalcularPotencialGanho(LoteMinerio lote)
        {
            var classificacao = ClassificadorQualidade.ObterClassificacao(lote);
            
            if (classificacao == "Premium")
                return 0; // Já é premium

            var valorAtual = CalcularValorTotal(lote);
            var valorSePremium = PRECO_PREMIUM * lote.Toneladas;

            return valorSePremium - valorAtual;
        }

        /// <summary>
        /// Retorna os preços base configurados
        /// </summary>
        public static object ObterTabelaPrecos()
        {
            return new
            {
                Premium = new { Valor = PRECO_PREMIUM, Moeda = "USD" },
                Padrao = new { Valor = PRECO_PADRAO, Moeda = "USD" },
                Baixa = new { Valor = PRECO_BAIXA, Moeda = "USD" }
            };
        }

        /// <summary>
        /// Calcula o preço médio ponderado de uma lista de lotes
        /// </summary>
        public static decimal CalcularPrecoMedioPonderado(List<LoteMinerio> lotes)
        {
            if (lotes == null || !lotes.Any())
                return 0;

            var valorTotalGeral = lotes.Sum(l => CalcularValorTotal(l));
            var toneladasTotais = lotes.Sum(l => l.Toneladas);

            return toneladasTotais > 0 ? valorTotalGeral / toneladasTotais : 0;
        }
    }
}