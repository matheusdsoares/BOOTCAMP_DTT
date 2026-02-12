namespace MinhaApi.Services
{
    using MinhaApi.Models;

    public static class ClassificadorQualidade
    {
        /// <summary>
        /// Classifica a qualidade do minério baseado em Fe, Umidade e SiO₂
        /// </summary>
        /// <param name="lote">Lote de minério a ser classificado</param>
        /// <returns>String com a classificação: "Premium", "Padrão" ou "Baixa"</returns>
        public static string ObterClassificacao(LoteMinerio lote)
        {
            // Critérios para Premium
            bool isPremium = lote.TeorFe >= 65m && 
                            lote.Umidade < 8m && 
                            (lote.SiO2 == null || lote.SiO2 < 3m);

            if (isPremium)
                return "Premium";

            // Critérios para Baixa Qualidade
            bool isBaixa = lote.TeorFe < 60m || 
                          lote.Umidade > 10m || 
                          (lote.SiO2 != null && lote.SiO2 > 5m);

            if (isBaixa)
                return "Baixa";

            // Caso contrário, é Padrão
            return "Padrão";
        }

        /// <summary>
        /// Retorna a classificação como enum
        /// </summary>
        /// <param name="lote">Lote de minério a ser classificado</param>
        /// <returns>Enum ClassificacaoQualidade</returns>
        public static ClassificacaoQualidade ObterClassificacaoEnum(LoteMinerio lote)
        {
            string classificacao = ObterClassificacao(lote);
            
            return classificacao switch
            {
                "Premium" => ClassificacaoQualidade.Premium,
                "Baixa" => ClassificacaoQualidade.Baixa,
                _ => ClassificacaoQualidade.Padrao
            };
        }

        /// <summary>
        /// Verifica se o lote atende aos critérios Premium
        /// </summary>
        public static bool IsPremium(LoteMinerio lote)
        {
            return lote.TeorFe >= 65m && 
                   lote.Umidade < 8m && 
                   (lote.SiO2 == null || lote.SiO2 < 3m);
        }

        /// <summary>
        /// Verifica se o lote é de baixa qualidade
        /// </summary>
        public static bool IsBaixa(LoteMinerio lote)
        {
            return lote.TeorFe < 60m || 
                   lote.Umidade > 10m || 
                   (lote.SiO2 != null && lote.SiO2 > 5m);
        }
    }
}