using Microsoft.AspNetCore.Mvc;
using MinhaApi.Models;
using MinhaApi.Services;

namespace MinhaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PenalidadesController : ControllerBase
    {
        /// <summary>
        /// GET: api/Penalidades/lote/1
        /// Calcula a penalidade por umidade de um lote específico
        /// </summary>
        [HttpGet("lote/{id}")]
        public ActionResult<object> CalcularPenalidadeLote(int id)
        {
            var lote = BuscarLotePorId(id);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {id} não encontrado" });

            var resultado = CalculadorPenalidades.CalcularPenalidade(lote);
            var custoPorPonto = CalculadorPenalidades.CalcularCustoPorPontoPercentual(lote);

            return Ok(new
            {
                Lote = new
                {
                    lote.Id,
                    lote.CodigoLote,
                    lote.MinaOrigem,
                    lote.Toneladas
                },
                Umidade = new
                {
                    Atual = resultado.UmidadeAtual,
                    Ideal = resultado.UmidadeIdeal,
                    Excesso = resultado.ExcessoUmidade,
                    Classificacao = resultado.ClassificacaoUmidade
                },
                ImpactoFinanceiro = new
                {
                    ValorOriginal = resultado.ValorOriginal,
                    PercentualPenalidade = resultado.PercentualPenalidade,
                    ValorPenalidade = resultado.ValorPenalidade,
                    ValorFinal = resultado.ValorFinal,
                    PerdaFinanceira = resultado.PerdaFinanceira,
                    CustoPorPontoPercentual = custoPorPonto,
                    Moeda = "USD"
                },
                Analise = new
                {
                    TemPenalidade = resultado.TemPenalidade,
                    Recomendacao = resultado.Recomendacao
                }
            });
        }

        /// <summary>
        /// GET: api/Penalidades/tabela
        /// Retorna a tabela de penalidades configurada
        /// </summary>
        [HttpGet("tabela")]
        public ActionResult<object> ObterTabelaPenalidades()
        {
            var tabela = CalculadorPenalidades.ObterTabelaPenalidades();
            return Ok(tabela);
        }

        /// <summary>
        /// POST: api/Penalidades/simular-reducao
        /// Simula economia ao reduzir a umidade
        /// </summary>
        [HttpPost("simular-reducao")]
        public ActionResult<object> SimularReducaoUmidade([FromBody] SimulacaoReducaoRequest request)
        {
            var lote = BuscarLotePorId(request.LoteId);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {request.LoteId} não encontrado" });

            if (request.UmidadeAlvo >= lote.Umidade)
            {
                return BadRequest(new 
                { 
                    mensagem = "A umidade alvo deve ser menor que a umidade atual",
                    umidadeAtual = lote.Umidade,
                    umidadeAlvo = request.UmidadeAlvo
                });
            }

            var penalAtual = CalculadorPenalidades.CalcularPenalidade(lote);
            
            // Simular com umidade reduzida
            var loteSimulado = new LoteMinerio
            {
                CodigoLote = lote.CodigoLote,
                TeorFe = lote.TeorFe,
                Umidade = request.UmidadeAlvo,
                SiO2 = lote.SiO2,
                Toneladas = lote.Toneladas
            };

            var penalSimulada = CalculadorPenalidades.CalcularPenalidade(loteSimulado);
            var economia = CalculadorPenalidades.CalcularEconomiaPotencial(lote, request.UmidadeAlvo);

            return Ok(new
            {
                Simulacao = new
                {
                    UmidadeAtual = lote.Umidade,
                    UmidadeAlvo = request.UmidadeAlvo,
                    ReducaoUmidade = lote.Umidade - request.UmidadeAlvo
                },
                Cenario  = new
                {
                    Classificacao = penalAtual.ClassificacaoUmidade,
                    PercentualPenalidade = penalAtual.PercentualPenalidade,
                    PerdaFinanceira = penalAtual.PerdaFinanceira,
                    ValorFinal = penalAtual.ValorFinal
                },
                CenarioComMelhoria = new
                {
                    Classificacao = penalSimulada.ClassificacaoUmidade,
                    PercentualPenalidade = penalSimulada.PercentualPenalidade,
                    PerdaFinanceira = penalSimulada.PerdaFinanceira,
                    ValorFinal = penalSimulada.ValorFinal
                },
                Beneficio = new
                {
                    EconomiaFinanceira = economia,
                    PercentualEconomia = penalAtual.ValorOriginal > 0 
                        ? Math.Round(economia / penalAtual.ValorOriginal * 100, 2) 
                        : 0,
                    ROI = request.CustoSecagem.HasValue && request.CustoSecagem > 0
                        ? Math.Round((economia - request.CustoSecagem.Value) / request.CustoSecagem.Value * 100, 2)
                        : (decimal?)null,
                    LucroLiquido = request.CustoSecagem.HasValue
                        ? economia - request.CustoSecagem.Value
                        : (decimal?)null,
                    Recomendacao = economia > (request.CustoSecagem ?? 0)
                        ? "✅ Vale a pena investir na secagem"
                        : "❌ Custo de secagem não compensa a economia"
                },
                Moeda = "USD"
            });
        }

        /// <summary>
        /// GET: api/Penalidades/relatorio-geral
        /// Gera relatório de impacto da umidade em todos os lotes
        /// </summary>
        [HttpGet("relatorio-geral")]
        public ActionResult<object> ObterRelatorioGeral()
        {
            var todosLotes = ObterTodosLotes();
            var relatorio = CalculadorPenalidades.CalcularImpactoGeral(todosLotes);

            return Ok(new
            {
                DataRelatorio = DateTime.Now,
                Resumo = new
                {
                    relatorio.TotalLotes,
                    relatorio.LotesComPenalidade,
                    relatorio.LotesSemPenalidade,
                    PercentualAfetados = $"{relatorio.PercentualLotesAfetados}%"
                },
                ImpactoFinanceiro = new
                {
                    ValorTotalOriginal = relatorio.ValorTotalOriginal,
                    TotalPerdas = relatorio.TotalPerdas,
                    PercentualPerda = $"{relatorio.PercentualPerda}%",
                    Moeda = "USD"
                },
                Qualidade = new
                {
                    UmidadeMedia = relatorio.UmidadeMedia,
                    StatusGeral = relatorio.UmidadeMedia <= 8 ? "Excelente" :
                                  relatorio.UmidadeMedia <= 10 ? "Bom" :
                                  relatorio.UmidadeMedia <= 12 ? "Atenção" : "Crítico"
                },
                DetalhamentoLotes = relatorio.DetalhesPorLote
            });
        }

        /// <summary>
        /// GET: api/Penalidades/lotes-criticos
        /// Retorna lotes com umidade crítica que necessitam ação urgente
        /// </summary>
        [HttpGet("lotes-criticos")]
        public ActionResult<object> ObterLotesCriticos()
        {
            var todosLotes = ObterTodosLotes();
            
            var lotesCriticos = todosLotes
                .Select(lote => new
                {
                    Lote = lote,
                    Penalidade = CalculadorPenalidades.CalcularPenalidade(lote)
                })
                .Where(x => x.Penalidade.ClassificacaoUmidade == "Crítica" || 
                           x.Penalidade.ClassificacaoUmidade == "Alta")
                .OrderByDescending(x => x.Penalidade.PerdaFinanceira)
                .Select(x => new
                {
                    x.Lote.Id,
                    x.Lote.CodigoLote,
                    x.Lote.MinaOrigem,
                    x.Lote.Toneladas,
                    Umidade = x.Lote.Umidade,
                    x.Penalidade.ClassificacaoUmidade,
                    x.Penalidade.PercentualPenalidade,
                    x.Penalidade.PerdaFinanceira,
                    x.Penalidade.Recomendacao,
                    Prioridade = x.Penalidade.ClassificacaoUmidade == "Crítica" ? "🔴 URGENTE" : "🟡 ALTA"
                })
                .ToList();

            return Ok(new
            {
                TotalLotesCriticos = lotesCriticos.Count,
                PerdaTotalEstimada = lotesCriticos.Sum(l => l.PerdaFinanceira),
                AlertaNivel = lotesCriticos.Any(l => l.Prioridade.Contains("URGENTE")) 
                    ? "🔴 CRÍTICO - Ação imediata necessária" 
                    : "🟡 ATENÇÃO - Monitoramento necessário",
                Lotes = lotesCriticos
            });
        }

        /// <summary>
        /// GET: api/Penalidades/comparativo-minas
        /// Compara penalidades por umidade entre diferentes minas
        /// </summary>
        [HttpGet("comparativo-minas")]
        public ActionResult<object> ObterComparativoMinas()
        {
            var todosLotes = ObterTodosLotes();

            var comparativo = todosLotes
                .GroupBy(l => l.MinaOrigem)
                .Select(g =>
                {
                    var lotesGrupo = g.ToList();
                    var relatorio = CalculadorPenalidades.CalcularImpactoGeral(lotesGrupo);

                    return new
                    {
                        Mina = g.Key,
                        TotalLotes = lotesGrupo.Count,
                        UmidadeMedia = relatorio.UmidadeMedia,
                        LotesComPenalidade = relatorio.LotesComPenalidade,
                        PercentualAfetados = relatorio.PercentualLotesAfetados,
                        TotalPerdas = relatorio.TotalPerdas,
                        PercentualPerda = relatorio.PercentualPerda,
                        Classificacao = relatorio.UmidadeMedia <= 8 ? "⭐ Excelente" :
                                       relatorio.UmidadeMedia <= 10 ? "✅ Bom" :
                                       relatorio.UmidadeMedia <= 12 ? "⚠️ Atenção" : "🔴 Crítico"
                    };
                })
                .OrderBy(x => x.UmidadeMedia)
                .ToList();

            return Ok(new
            {
                TotalMinas = comparativo.Count,
                ComparativoMinas = comparativo,
                MelhorDesempenho = comparativo.FirstOrDefault(),
                PiorDesempenho = comparativo.LastOrDefault()
            });
        }

        // ===== MÉTODOS AUXILIARES =====

        private LoteMinerio? BuscarLotePorId(int id)
        {
            var lotes = ObterTodosLotes();
            return lotes.FirstOrDefault(l => l.Id == id);
        }

        private List<LoteMinerio> ObterTodosLotes()
        {
            // Simulação - substituir por consulta real ao banco
            return new List<LoteMinerio>
            {
                new LoteMinerio
                {
                    Id = 1,
                    CodigoLote = "MNA-2026-000123",
                    MinaOrigem = "Carajás N4E",
                    TeorFe = 67.5m,
                    Umidade = 7.2m,  // Ideal - sem penalidade
                    SiO2 = 2.5m,
                    Toneladas = 15000
                },
                new LoteMinerio
                {
                    Id = 2,
                    CodigoLote = "MNA-2026-000124",
                    MinaOrigem = "Carajás S11D",
                    TeorFe = 62.0m,
                    Umidade = 9.0m,  // Tolerável - penalidade leve
                    SiO2 = 4.0m,
                    Toneladas = 12000
                },
                new LoteMinerio
                {
                    Id = 3,
                    CodigoLote = "MNA-2026-000125",
                    MinaOrigem = "Minas Gerais",
                    TeorFe = 58.0m,
                    Umidade = 11.5m,  // Alta - penalidade moderada
                    SiO2 = 6.0m,
                    Toneladas = 8000
                },
                new LoteMinerio
                {
                    Id = 4,
                    CodigoLote = "MNA-2026-000126",
                    MinaOrigem = "Carajás N4E",
                    TeorFe = 64.0m,
                    Umidade = 13.5m,  // Severa - penalidade severa
                    SiO2 = 3.5m,
                    Toneladas = 10000
                },
                new LoteMinerio
                {
                    Id = 5,
                    CodigoLote = "MNA-2026-000127",
                    MinaOrigem = "Minas Gerais",
                    TeorFe = 60.0m,
                    Umidade = 16.0m,  // Crítica - penalidade crítica
                    SiO2 = 5.0m,
                    Toneladas = 5000
                }
            };
        }
    }

    // Classe de Request
    public class SimulacaoReducaoRequest
    {
        public int LoteId { get; set; }
        public decimal UmidadeAlvo { get; set; }
        public decimal? CustoSecagem { get; set; }  // Custo estimado do processo de secagem
    }
}
