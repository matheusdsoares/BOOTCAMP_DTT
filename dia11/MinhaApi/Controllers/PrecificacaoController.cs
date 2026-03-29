using Microsoft.AspNetCore.Mvc;
using MinhaApi.Models;
using MinhaApi.Services;

namespace MinhaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrecificacaoController : ControllerBase
    {
        /// <summary>
        /// GET: api/Precificacao/lote/1
        /// Obtém a precificação completa de um lote específico
        /// </summary>
        [HttpGet("lote/{id}")]
        public ActionResult<object> ObterPrecificacaoLote(int id)
        {
            var lote = BuscarLotePorId(id);

            if (lote == null)
                return NotFound(new { mensagem = $"Lote com ID {id} não encontrado" });

            var classificacao = ClassificadorQualidade.ObterClassificacao(lote);
            var precoPorTonelada = CalculadorPreco.CalcularPrecoPorTonelada(lote);
            var valorTotal = CalculadorPreco.CalcularValorTotal(lote);
            var potencialGanho = CalculadorPreco.CalcularPotencialGanho(lote);

            return Ok(new
            {
                Lote = new
                {
                    lote.Id,
                    lote.CodigoLote,
                    lote.MinaOrigem,
                    lote.Toneladas
                },
                Qualidade = new
                {
                    lote.TeorFe,
                    lote.Umidade,
                    lote.SiO2,
                    Classificacao = classificacao
                },
                Precificacao = new
                {
                    PrecoPorTonelada = precoPorTonelada,
                    ValorTotal = valorTotal,
                    PotencialGanhoSePremium = potencialGanho,
                    Moeda = "USD"
                }
            });
        }

        /// <summary>
        /// GET: api/Precificacao/tabela-precos
        /// Retorna a tabela de preços base por classificação
        /// </summary>
        [HttpGet("tabela-precos")]
        public ActionResult<object> ObterTabelaPrecos()
        {
            var tabela = CalculadorPreco.ObterTabelaPrecos();
            return Ok(tabela);
        }

        /// <summary>
        /// GET: api/Precificacao/resumo-geral
        /// Retorna um resumo de todos os lotes com valores totais
        /// </summary>
        [HttpGet("resumo-geral")]
        public ActionResult<object> ObterResumoGeral()
        {
            var todosLotes = ObterTodosLotes();

            var lotesComPreco = todosLotes.Select(lote => new
            {
                lote.Id,
                lote.CodigoLote,
                lote.MinaOrigem,
                lote.Toneladas,
                Classificacao = ClassificadorQualidade.ObterClassificacao(lote),
                PrecoPorTonelada = CalculadorPreco.CalcularPrecoPorTonelada(lote),
                ValorTotal = CalculadorPreco.CalcularValorTotal(lote)
            }).ToList();

            var valorTotalGeral = lotesComPreco.Sum(l => l.ValorTotal);
            var toneladasTotais = lotesComPreco.Sum(l => l.Toneladas);
            var precoMedioPonderado = CalculadorPreco.CalcularPrecoMedioPonderado(todosLotes);

            return Ok(new
            {
                TotalLotes = lotesComPreco.Count,
                ToneladasTotais = toneladasTotais,
                ValorTotalEstoque = valorTotalGeral,
                PrecoMedioPonderado = precoMedioPonderado,
                Moeda = "USD",
                Lotes = lotesComPreco
            });
        }

        /// <summary>
        /// GET: api/Precificacao/por-classificacao
        /// Agrupa valores por classificação de qualidade
        /// </summary>
        [HttpGet("por-classificacao")]
        public ActionResult<object> ObterValoresPorClassificacao()
        {
            var todosLotes = ObterTodosLotes();

            var lotesPremium = todosLotes.Where(l => ClassificadorQualidade.IsPremium(l)).ToList();
            var lotesPadrao = todosLotes.Where(l => ClassificadorQualidade.ObterClassificacao(l) == "Padrão").ToList();
            var lotesBaixa = todosLotes.Where(l => ClassificadorQualidade.IsBaixa(l)).ToList();

            return Ok(new
            {
                Premium = new
                {
                    QuantidadeLotes = lotesPremium.Count,
                    Toneladas = lotesPremium.Sum(l => l.Toneladas),
                    ValorTotal = lotesPremium.Sum(l => CalculadorPreco.CalcularValorTotal(l)),
                    PrecoPorTonelada = lotesPremium.Any() ? CalculadorPreco.CalcularPrecoPorTonelada(lotesPremium.First()) : 0
                },
                Padrao = new
                {
                    QuantidadeLotes = lotesPadrao.Count,
                    Toneladas = lotesPadrao.Sum(l => l.Toneladas),
                    ValorTotal = lotesPadrao.Sum(l => CalculadorPreco.CalcularValorTotal(l)),
                    PrecoPorTonelada = lotesPadrao.Any() ? CalculadorPreco.CalcularPrecoPorTonelada(lotesPadrao.First()) : 0
                },
                Baixa = new
                {
                    QuantidadeLotes = lotesBaixa.Count,
                    Toneladas = lotesBaixa.Sum(l => l.Toneladas),
                    ValorTotal = lotesBaixa.Sum(l => CalculadorPreco.CalcularValorTotal(l)),
                    PrecoPorTonelada = lotesBaixa.Any() ? CalculadorPreco.CalcularPrecoPorTonelada(lotesBaixa.First()) : 0
                },
                Moeda = "USD"
            });
        }

        /// <summary>
        /// GET: api/Precificacao/potencial-ganho
        /// Calcula quanto poderia ganhar se todos os lotes fossem Premium
        /// </summary>
        [HttpGet("potencial-ganho")]
        public ActionResult<object> CalcularPotencialGanho()
        {
            var todosLotes = ObterTodosLotes();

            var valorAtual = todosLotes.Sum(l => CalculadorPreco.CalcularValorTotal(l));
            var potencialGanhoTotal = todosLotes.Sum(l => CalculadorPreco.CalcularPotencialGanho(l));
            var valorPotencial = valorAtual + potencialGanhoTotal;

            var lotesNaoPremium = todosLotes
                .Where(l => !ClassificadorQualidade.IsPremium(l))
                .Select(l => new
                {
                    l.CodigoLote,
                    l.Toneladas,
                    ClassificacaoAtual = ClassificadorQualidade.ObterClassificacao(l),
                    ValorAtual = CalculadorPreco.CalcularValorTotal(l),
                    PotencialGanho = CalculadorPreco.CalcularPotencialGanho(l)
                })
                .ToList();

            return Ok(new
            {
                ValorAtualEstoque = valorAtual,
                ValorPotencialSeTodosPremium = valorPotencial,
                PotencialGanhoTotal = potencialGanhoTotal,
                PercentualAumento = valorAtual > 0 ? Math.Round((potencialGanhoTotal / valorAtual) * 100, 2) : 0,
                Moeda = "USD",
                LotesComPotencial = lotesNaoPremium
            });
        }

        /// <summary>
        /// POST: api/Precificacao/simular
        /// Simula a precificação de um lote sem salvá-lo
        /// </summary>
        [HttpPost("simular")]
        public ActionResult<object> SimularPrecificacao([FromBody] LoteMinerio lote)
        {
            if (lote == null)
                return BadRequest(new { mensagem = "Dados do lote inválidos" });

            var detalhamento = CalculadorPreco.ObterDetalhamentoPrecificacao(lote);
            var classificacao = ClassificadorQualidade.ObterClassificacao(lote);
            var potencialGanho = CalculadorPreco.CalcularPotencialGanho(lote);

            return Ok(new
            {
                Simulacao = detalhamento,
                Analise = new
                {
                    Classificacao = classificacao,
                    IsPremium = ClassificadorQualidade.IsPremium(lote),
                    PotencialGanhoSeMelhorar = potencialGanho,
                    Recomendacao = ObterRecomendacaoMelhoria(lote)
                }
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
            // Simulação - substitua pela busca real no banco
            return new List<LoteMinerio>
            {
                new LoteMinerio
                {
                    Id = 1,
                    CodigoLote = "MNA-2026-000123",
                    MinaOrigem = "Carajás N4E",
                    TeorFe = 67.5m,
                    Umidade = 7.2m,
                    SiO2 = 2.5m,
                    Toneladas = 15000,
                    Status = StatusLote.EmEstoque,
                    DataProducao = DateTime.Now.AddDays(-5)
                },
                new LoteMinerio
                {
                    Id = 2,
                    CodigoLote = "MNA-2026-000124",
                    MinaOrigem = "Carajás S11D",
                    TeorFe = 62.0m,
                    Umidade = 9.0m,
                    SiO2 = 4.0m,
                    Toneladas = 12000,
                    Status = StatusLote.EmTransporte,
                    DataProducao = DateTime.Now.AddDays(-3)
                },
                new LoteMinerio
                {
                    Id = 3,
                    CodigoLote = "MNA-2026-000125",
                    MinaOrigem = "Minas Gerais",
                    TeorFe = 58.0m,
                    Umidade = 11.5m,
                    SiO2 = 6.0m,
                    Toneladas = 8000,
                    Status = StatusLote.EmEstoque,
                    DataProducao = DateTime.Now.AddDays(-1)
                }
            };
        }

        private string ObterRecomendacaoMelhoria(LoteMinerio lote)
        {
            var classificacao = ClassificadorQualidade.ObterClassificacao(lote);

            if (classificacao == "Premium")
                return "Lote já está na melhor classificação possível.";

            var problemas = new List<string>();

            if (lote.TeorFe < 65m)
                problemas.Add($"Aumentar Teor de Fe de {lote.TeorFe}% para 65%+");

            if (lote.Umidade >= 8m)
                problemas.Add($"Reduzir Umidade de {lote.Umidade}% para menos de 8%");

            if (lote.SiO2 != null && lote.SiO2 >= 3m)
                problemas.Add($"Reduzir SiO₂ de {lote.SiO2}% para menos de 3%");

            return problemas.Any() 
                ? $"Para alcançar classificação Premium: {string.Join(", ", problemas)}" 
                : "Lote próximo da classificação Premium.";
        }
    }
}
