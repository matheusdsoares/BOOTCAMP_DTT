namespace MinhaApi.Services
{
    using MinhaApi.Models;
    using System.Text.Json;

    public static class GerenciadorHistorico
    {
        /// <summary>
        /// Registra uma nova movimentação no histórico
        /// </summary>
        public static HistoricoMovimentacao RegistrarMovimentacao(
            int loteId,
            string codigoLote,
            TipoMovimentacao tipo,
            string descricao,
            string? usuario = null,
            string? observacoes = null)
        {
            var historico = new HistoricoMovimentacao
            {
                LoteId = loteId,
                CodigoLote = codigoLote,
                TipoMovimentacao = tipo,
                Descricao = descricao,
                DataHoraMovimentacao = DateTime.Now,
                UsuarioResponsavel = usuario,
                Observacoes = observacoes
            };

            // Aqui você salvaria no banco de dados
            // _context.HistoricoMovimentacoes.Add(historico);
            // _context.SaveChanges();

            return historico;
        }

        /// <summary>
        /// Registra mudança de status
        /// </summary>
        public static HistoricoMovimentacao RegistrarMudancaStatus(
            LoteMinerio lote,
            StatusLote statusAntigo,
            StatusLote statusNovo,
            string? usuario = null,
            string? observacoes = null)
        {
            var historico = new HistoricoMovimentacao
            {
                LoteId = lote.Id,
                CodigoLote = lote.CodigoLote,
                TipoMovimentacao = TipoMovimentacao.AlteracaoStatus,
                Descricao = $"Status alterado de {statusAntigo} para {statusNovo}",
                StatusAnterior = statusAntigo.ToString(),
                StatusNovo = statusNovo.ToString(),
                DataHoraMovimentacao = DateTime.Now,
                UsuarioResponsavel = usuario,
                Observacoes = observacoes
            };

            // Salvar no banco
            return historico;
        }

        /// <summary>
        /// Registra mudança de localização
        /// </summary>
        public static HistoricoMovimentacao RegistrarMudancaLocalizacao(
            LoteMinerio lote,
            string localizacaoAntiga,
            string localizacaoNova,
            string? usuario = null,
            string? observacoes = null)
        {
            var historico = new HistoricoMovimentacao
            {
                LoteId = lote.Id,
                CodigoLote = lote.CodigoLote,
                TipoMovimentacao = TipoMovimentacao.AlteracaoLocalizacao,
                Descricao = $"Movimentado de '{localizacaoAntiga}' para '{localizacaoNova}'",
                LocalizacaoAnterior = localizacaoAntiga,
                LocalizacaoNova = localizacaoNova,
                DataHoraMovimentacao = DateTime.Now,
                UsuarioResponsavel = usuario,
                Observacoes = observacoes
            };

            // Salvar no banco
            return historico;
        }

        /// <summary>
        /// Registra criação de um novo lote
        /// </summary>
        public static HistoricoMovimentacao RegistrarCriacaoLote(
            LoteMinerio lote,
            string? usuario = null)
        {
            var historico = new HistoricoMovimentacao
            {
                LoteId = lote.Id,
                CodigoLote = lote.CodigoLote,
                TipoMovimentacao = TipoMovimentacao.CriacaoLote,
                Descricao = $"Lote criado na mina {lote.MinaOrigem}",
                StatusNovo = lote.Status.ToString(),
                LocalizacaoNova = lote.LocalizacaoAtual,
                DataHoraMovimentacao = DateTime.Now,
                UsuarioResponsavel = usuario,
                DadosAdicionais = JsonSerializer.Serialize(new
                {
                    lote.TeorFe,
                    lote.Umidade,
                    lote.SiO2,
                    lote.Toneladas
                })
            };

            // Salvar no banco
            return historico;
        }

        /// <summary>
        /// Registra embarque de lote
        /// </summary>
        public static HistoricoMovimentacao RegistrarEmbarque(
            LoteMinerio lote,
            string destino,
            string? numeroNavio = null,
            string? usuario = null)
        {
            var historico = new HistoricoMovimentacao
            {
                LoteId = lote.Id,
                CodigoLote = lote.CodigoLote,
                TipoMovimentacao = TipoMovimentacao.Embarque,
                Descricao = $"Lote embarcado com destino a {destino}",
                StatusAnterior = lote.Status.ToString(),
                StatusNovo = StatusLote.Embarcado.ToString(),
                LocalizacaoNova = numeroNavio ?? destino,
                DataHoraMovimentacao = DateTime.Now,
                UsuarioResponsavel = usuario,
                DadosAdicionais = JsonSerializer.Serialize(new
                {
                    Destino = destino,
                    NumeroNavio = numeroNavio,
                    Toneladas = lote.Toneladas
                })
            };

            // Salvar no banco
            return historico;
        }

        /// <summary>
        /// Busca histórico de um lote específico
        /// </summary>
        public static List<HistoricoMovimentacao> ObterHistoricoPorLote(int loteId)
        {
            // Simulação - substituir por consulta ao banco
            // return _context.HistoricoMovimentacoes
            //     .Where(h => h.LoteId == loteId)
            //     .OrderByDescending(h => h.DataHoraMovimentacao)
            //     .ToList();

            return new List<HistoricoMovimentacao>();
        }

        /// <summary>
        /// Busca histórico por período
        /// </summary>
        public static List<HistoricoMovimentacao> ObterHistoricoPorPeriodo(
            DateTime dataInicio,
            DateTime dataFim)
        {
            // Consulta ao banco com filtro de data
            return new List<HistoricoMovimentacao>();
        }

        /// <summary>
        /// Busca histórico por tipo de movimentação
        /// </summary>
        public static List<HistoricoMovimentacao> ObterHistoricoPorTipo(
            TipoMovimentacao tipo)
        {
            // Consulta ao banco com filtro de tipo
            return new List<HistoricoMovimentacao>();
        }

        /// <summary>
        /// Gera relatório de rastreabilidade completa do lote
        /// </summary>
        public static object GerarRelatorioRastreabilidade(int loteId)
        {
            var historico = ObterHistoricoPorLote(loteId);

            return new
            {
                LoteId = loteId,
                TotalMovimentacoes = historico.Count,
                PrimeiraMovimentacao = historico.LastOrDefault()?.DataHoraMovimentacao,
                UltimaMovimentacao = historico.FirstOrDefault()?.DataHoraMovimentacao,
                Movimentacoes = historico.Select(h => new
                {
                    h.DataHoraMovimentacao,
                    h.TipoMovimentacao,
                    h.Descricao,
                    h.LocalizacaoAnterior,
                    h.LocalizacaoNova,
                    h.StatusAnterior,
                    h.StatusNovo,
                    h.UsuarioResponsavel,
                    h.Observacoes
                })
            };
        }
    }
}