using Microsoft.AspNetCore.Mvc;
using MinhaApi.Models;
using MinhaApi.Services;

namespace MinhaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtraLotesMinerioController : ControllerBase
    {
        /// <summary>
        /// GET: api/ExtraLotesMinerio/classificacao/5
        /// Obtém a classificação de qualidade de um lote específico
        /// </summary>
        [HttpGet("classificacao/{id}")]
        public ActionResult<object> ObterClassificacaoLote(int id)
        {
            // Aqui você buscaria o lote do banco de dados pelo ID
            // Por enquanto, vou simular um lote de exemplo
            var lote = BuscarLotePorId(id);

            if (lote == null)
                return NotFound(new { mensagem = $"Lote com ID {id} não encontrado" });

            var classificacao = ClassificadorQualidade.ObterClassificacao(lote);
            var classificacaoEnum = ClassificadorQualidade.ObterClassificacaoEnum(lote);

            return Ok(new
            {
                lote.Id,
                lote.CodigoLote,
                lote.MinaOrigem,
                Parametros = new
                {
                    lote.TeorFe,
                    lote.Umidade,
                    lote.SiO2
                },
                ClassificacaoTexto = classificacao,
                ClassificacaoNivel = classificacaoEnum,
                IsPremium = ClassificadorQualidade.IsPremium(lote),
                IsBaixa = ClassificadorQualidade.IsBaixa(lote)
            });
        }

        /// <summary>
        /// GET: api/ExtraLotesMinerio/listar-com-classificacao
        /// Lista todos os lotes com suas respectivas classificações
        /// </summary>
        [HttpGet("listar-com-classificacao")]
        public ActionResult<IEnumerable<object>> ListarLotesComClassificacao()
        {
            var todosLotes = ObterTodosLotes();

            var lotesComClassificacao = todosLotes.Select(lote => new
            {
                lote.Id,
                lote.CodigoLote,
                lote.MinaOrigem,
                lote.TeorFe,
                lote.Umidade,
                lote.SiO2,
                lote.Toneladas,
                lote.Status,
                Classificacao = ClassificadorQualidade.ObterClassificacao(lote)
            });

            return Ok(lotesComClassificacao);
        }

        /// <summary>
        /// GET: api/ExtraLotesMinerio/por-qualidade/Premium
        /// Filtra lotes por classificação de qualidade
        /// </summary>
        [HttpGet("por-qualidade/{qualidade}")]
        public ActionResult<IEnumerable<LoteMinerio>> FiltrarPorQualidade(string qualidade)
        {
            // Validar se a qualidade informada é válida
            if (qualidade != "Premium" && qualidade != "Padrão" && qualidade != "Baixa")
            {
                return BadRequest(new { mensagem = "Qualidade inválida. Use: Premium, Padrão ou Baixa" });
            }

            var todosLotes = ObterTodosLotes();

            var lotesFiltrados = todosLotes
                .Where(l => ClassificadorQualidade.ObterClassificacao(l) == qualidade)
                .ToList();

            return Ok(new
            {
                QualidadeBuscada = qualidade,
                TotalEncontrado = lotesFiltrados.Count,
                Lotes = lotesFiltrados
            });
        }

        /// <summary>
        /// GET: api/ExtraLotesMinerio/premium
        /// Retorna apenas lotes Premium
        /// </summary>
        [HttpGet("premium")]
        public ActionResult<IEnumerable<LoteMinerio>> ObterLotesPremium()
        {
            var todosLotes = ObterTodosLotes();
            var lotesPremium = todosLotes.Where(l => ClassificadorQualidade.IsPremium(l)).ToList();

            return Ok(new
            {
                Total = lotesPremium.Count,
                Lotes = lotesPremium
            });
        }

        /// <summary>
        /// GET: api/ExtraLotesMinerio/estatisticas
        /// Retorna estatísticas de qualidade de todos os lotes
        /// </summary>
        [HttpGet("estatisticas")]
        public ActionResult<object> ObterEstatisticasQualidade()
        {
            var todosLotes = ObterTodosLotes();

            var totalLotes = todosLotes.Count;
            var lotesPremium = todosLotes.Count(l => ClassificadorQualidade.IsPremium(l));
            var lotesPadrao = todosLotes.Count(l => ClassificadorQualidade.ObterClassificacao(l) == "Padrão");
            var lotesBaixa = todosLotes.Count(l => ClassificadorQualidade.IsBaixa(l));

            var toneladasPremium = todosLotes
                .Where(l => ClassificadorQualidade.IsPremium(l))
                .Sum(l => l.Toneladas);

            var toneladasPadrao = todosLotes
                .Where(l => ClassificadorQualidade.ObterClassificacao(l) == "Padrão")
                .Sum(l => l.Toneladas);

            var toneladasBaixa = todosLotes
                .Where(l => ClassificadorQualidade.IsBaixa(l))
                .Sum(l => l.Toneladas);

            return Ok(new
            {
                TotalLotes = totalLotes,
                DistribuicaoPorQualidade = new
                {
                    Premium = new
                    {
                        Quantidade = lotesPremium,
                        Percentual = totalLotes > 0 ? Math.Round((decimal)lotesPremium / totalLotes * 100, 2) : 0,
                        Toneladas = toneladasPremium
                    },
                    Padrao = new
                    {
                        Quantidade = lotesPadrao,
                        Percentual = totalLotes > 0 ? Math.Round((decimal)lotesPadrao / totalLotes * 100, 2) : 0,
                        Toneladas = toneladasPadrao
                    },
                    Baixa = new
                    {
                        Quantidade = lotesBaixa,
                        Percentual = totalLotes > 0 ? Math.Round((decimal)lotesBaixa / totalLotes * 100, 2) : 0,
                        Toneladas = toneladasBaixa
                    }
                },
                ToneladasTotais = todosLotes.Sum(l => l.Toneladas)
            });
        }

        /// <summary>
        /// POST: api/ExtraLotesMinerio/classificar
        /// Classifica um lote enviado no body da requisição
        /// </summary>
        [HttpPost("classificar")]
        public ActionResult<object> ClassificarLote([FromBody] LoteMinerio lote)
        {
            if (lote == null)
                return BadRequest(new { mensagem = "Lote inválido" });

            var classificacao = ClassificadorQualidade.ObterClassificacao(lote);

            return Ok(new
            {
                LoteAnalisado = new
                {
                    lote.CodigoLote,
                    lote.TeorFe,
                    lote.Umidade,
                    lote.SiO2
                },
                ResultadoClassificacao = classificacao,
                Recomendacao = ObterRecomendacao(classificacao)
            });
        }

        // ===== MÉTODOS AUXILIARES =====
        // Substitua estes métodos pela sua lógica real de acesso ao banco de dados

        private LoteMinerio? BuscarLotePorId(int id)
        {
            // Simulação - substitua pela busca real no banco
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

        private string ObterRecomendacao(string classificacao)
        {
            return classificacao switch
            {
                "Premium" => "Minério de alta qualidade. Recomendado para exportação premium.",
                "Padrão" => "Minério de qualidade padrão. Adequado para mercado doméstico e exportação.",
                "Baixa" => "Minério de baixa qualidade. Recomendado beneficiamento antes da venda.",
                _ => "Classificação não identificada."
            };
        }
    }
}
