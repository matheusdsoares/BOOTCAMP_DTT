using Microsoft.AspNetCore.Mvc;
using MinhaApi.Models;
using MinhaApi.Services;

namespace MinhaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricoMovimentacaoController : ControllerBase
    {
        /// <summary>
        /// GET: api/HistoricoMovimentacao/lote/1
        /// Retorna todo o histórico de um lote específico
        /// </summary>
        [HttpGet("lote/{loteId}")]
        public ActionResult<IEnumerable<HistoricoMovimentacao>> ObterHistoricoPorLote(int loteId)
        {
            var historico = ObterHistoricoSimulado(loteId);

            if (!historico.Any())
                return NotFound(new { mensagem = $"Nenhum histórico encontrado para o lote ID {loteId}" });

            return Ok(new
            {
                LoteId = loteId,
                TotalMovimentacoes = historico.Count,
                Historico = historico.OrderByDescending(h => h.DataHoraMovimentacao)
            });
        }

        /// <summary>
        /// GET: api/HistoricoMovimentacao/rastreabilidade/1
        /// Retorna rastreabilidade completa do lote
        /// </summary>
        [HttpGet("rastreabilidade/{loteId}")]
        public ActionResult<object> ObterRastreabilidade(int loteId)
        {
            var historico = ObterHistoricoSimulado(loteId).OrderBy(h => h.DataHoraMovimentacao).ToList();

            if (!historico.Any())
                return NotFound(new { mensagem = $"Nenhum histórico encontrado para o lote ID {loteId}" });

            var lote = BuscarLotePorId(loteId);

            return Ok(new
            {
                Lote = lote != null ? new
                {
                    lote.Id,
                    lote.CodigoLote,
                    lote.MinaOrigem,
                    lote.Status,
                    lote.LocalizacaoAtual
                } : null,
                Rastreabilidade = new
                {
                    TotalMovimentacoes = historico.Count,
                    DataCriacao = historico.First().DataHoraMovimentacao,
                    UltimaAtualizacao = historico.Last().DataHoraMovimentacao,
                    TempoTotal = (historico.Last().DataHoraMovimentacao - historico.First().DataHoraMovimentacao).TotalDays,
                    LinhaDoTempo = historico.Select(h => new
                    {
                        h.DataHoraMovimentacao,
                        h.TipoMovimentacao,
                        h.Descricao,
                        Origem = h.LocalizacaoAnterior ?? h.StatusAnterior,
                        Destino = h.LocalizacaoNova ?? h.StatusNovo,
                        h.UsuarioResponsavel,
                        h.Observacoes
                    })
                }
            });
        }

        /// <summary>
        /// GET: api/HistoricoMovimentacao/por-periodo?dataInicio=2026-01-01&dataFim=2026-02-28
        /// Retorna histórico filtrado por período
        /// </summary>
        [HttpGet("por-periodo")]
        public ActionResult<IEnumerable<HistoricoMovimentacao>> ObterPorPeriodo(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            if (dataFim < dataInicio)
                return BadRequest(new { mensagem = "Data final não pode ser menor que data inicial" });

            var todosHistoricos = ObterTodosHistoricos();
            var historicoFiltrado = todosHistoricos
                .Where(h => h.DataHoraMovimentacao >= dataInicio && h.DataHoraMovimentacao <= dataFim)
                .OrderByDescending(h => h.DataHoraMovimentacao)
                .ToList();

            return Ok(new
            {
                Periodo = new
                {
                    DataInicio = dataInicio,
                    DataFim = dataFim
                },
                TotalMovimentacoes = historicoFiltrado.Count,
                Historico = historicoFiltrado
            });
        }

        /// <summary>
        /// GET: api/HistoricoMovimentacao/por-tipo/AlteracaoStatus
        /// Retorna histórico filtrado por tipo de movimentação
        /// </summary>
        [HttpGet("por-tipo/{tipo}")]
        public ActionResult<IEnumerable<HistoricoMovimentacao>> ObterPorTipo(TipoMovimentacao tipo)
        {
            var todosHistoricos = ObterTodosHistoricos();
            var historicoFiltrado = todosHistoricos
                .Where(h => h.TipoMovimentacao == tipo)
                .OrderByDescending(h => h.DataHoraMovimentacao)
                .ToList();

            return Ok(new
            {
                TipoMovimentacao = tipo.ToString(),
                TotalMovimentacoes = historicoFiltrado.Count,
                Historico = historicoFiltrado
            });
        }

        /// <summary>
        /// POST: api/HistoricoMovimentacao/registrar-status
        /// Registra uma mudança de status
        /// </summary>
        [HttpPost("registrar-status")]
        public ActionResult<HistoricoMovimentacao> RegistrarMudancaStatus(
            [FromBody] RegistroStatusRequest request)
        {
            var lote = BuscarLotePorId(request.LoteId);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {request.LoteId} não encontrado" });

            var statusAntigo = lote.Status;
            lote.Status = request.NovoStatus;

            var historico = GerenciadorHistorico.RegistrarMudancaStatus(
                lote,
                statusAntigo,
                request.NovoStatus,
                request.Usuario,
                request.Observacoes
            );

            // Aqui você salvaria no banco

            return CreatedAtAction(
                nameof(ObterHistoricoPorLote),
                new { loteId = lote.Id },
                historico
            );
        }

        /// <summary>
        /// POST: api/HistoricoMovimentacao/registrar-localizacao
        /// Registra uma mudança de localização
        /// </summary>
        [HttpPost("registrar-localizacao")]
        public ActionResult<HistoricoMovimentacao> RegistrarMudancaLocalizacao(
            [FromBody] RegistroLocalizacaoRequest request)
        {
            var lote = BuscarLotePorId(request.LoteId);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {request.LoteId} não encontrado" });

            var localizacaoAntiga = lote.LocalizacaoAtual;
            lote.LocalizacaoAtual = request.NovaLocalizacao;

            var historico = GerenciadorHistorico.RegistrarMudancaLocalizacao(
                lote,
                localizacaoAntiga,
                request.NovaLocalizacao,
                request.Usuario,
                request.Observacoes
            );

            // Salvar no banco

            return CreatedAtAction(
                nameof(ObterHistoricoPorLote),
                new { loteId = lote.Id },
                historico
            );
        }

        /// <summary>
        /// POST: api/HistoricoMovimentacao/registrar-embarque
        /// Registra embarque de um lote
        /// </summary>
        [HttpPost("registrar-embarque")]
        public ActionResult<HistoricoMovimentacao> RegistrarEmbarque(
            [FromBody] RegistroEmbarqueRequest request)
        {
            var lote = BuscarLotePorId(request.LoteId);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {request.LoteId} não encontrado" });

            var historico = GerenciadorHistorico.RegistrarEmbarque(
                lote,
                request.Destino,
                request.NumeroNavio,
                request.Usuario
            );

            lote.Status = StatusLote.Embarcado;
            lote.LocalizacaoAtual = request.NumeroNavio ?? request.Destino;

            // Salvar no banco

            return CreatedAtAction(
                nameof(ObterHistoricoPorLote),
                new { loteId = lote.Id },
                historico
            );
        }

        /// <summary>
        /// GET: api/HistoricoMovimentacao/estatisticas
        /// Retorna estatísticas gerais de movimentações
        /// </summary>
        [HttpGet("estatisticas")]
        public ActionResult<object> ObterEstatisticas()
        {
            var todosHistoricos = ObterTodosHistoricos();

            var estatisticasPorTipo = todosHistoricos
                .GroupBy(h => h.TipoMovimentacao)
                .Select(g => new
                {
                    Tipo = g.Key.ToString(),
                    Quantidade = g.Count(),
                    UltimaOcorrencia = g.Max(h => h.DataHoraMovimentacao)
                });

            var movimentacoesPorDia = todosHistoricos
                .GroupBy(h => h.DataHoraMovimentacao.Date)
                .Select(g => new
                {
                    Data = g.Key,
                    Quantidade = g.Count()
                })
                .OrderByDescending(x => x.Data)
                .Take(30);

            return Ok(new
            {
                TotalMovimentacoes = todosHistoricos.Count,
                MovimentacoesPorTipo = estatisticasPorTipo,
                MovimentacoesUltimos30Dias = movimentacoesPorDia,
                UsuariosMaisAtivos = todosHistoricos
                    .Where(h => !string.IsNullOrEmpty(h.UsuarioResponsavel))
                    .GroupBy(h => h.UsuarioResponsavel)
                    .Select(g => new
                    {
                        Usuario = g.Key,
                        Movimentacoes = g.Count()
                    })
                    .OrderByDescending(x => x.Movimentacoes)
                    .Take(5)
            });
        }

        // ===== MÉTODOS AUXILIARES E CLASSES DE REQUEST =====

        private LoteMinerio? BuscarLotePorId(int id)
        {
            // Substituir por busca real no banco
            return new LoteMinerio
            {
                Id = id,
                CodigoLote = $"MNA-2026-{id:000000}",
                MinaOrigem = "Carajás N4E",
                Status = StatusLote.EmEstoque,
                LocalizacaoAtual = "Pátio Carajás"
            };
        }

        private List<HistoricoMovimentacao> ObterHistoricoSimulado(int loteId)
        {
            // Simulação de histórico
            return new List<HistoricoMovimentacao>
            {
                new HistoricoMovimentacao
                {
                    Id = 1,
                    LoteId = loteId,
                    CodigoLote = $"MNA-2026-{loteId:000000}",
                    TipoMovimentacao = TipoMovimentacao.CriacaoLote,
                    Descricao = "Lote criado na mina Carajás N4E",
                    StatusNovo = "EmEstoque",
                    LocalizacaoNova = "Mina Carajás N4E",
                    DataHoraMovimentacao = DateTime.Now.AddDays(-10),
                    UsuarioResponsavel = "operador.mina@vale.com"
                },
                new HistoricoMovimentacao
                {
                    Id = 2,
                    LoteId = loteId,
                    CodigoLote = $"MNA-2026-{loteId:000000}",
                    TipoMovimentacao = TipoMovimentacao.AlteracaoLocalizacao,
                    Descricao = "Movimentado de 'Mina Carajás N4E' para 'Pátio Carajás'",
                    LocalizacaoAnterior = "Mina Carajás N4E",
                    LocalizacaoNova = "Pátio Carajás",
                    DataHoraMovimentacao = DateTime.Now.AddDays(-8),
                    UsuarioResponsavel = "logistica@vale.com",
                    Observacoes = "Transporte via caminhão CAT-789"
                },
                new HistoricoMovimentacao
                {
                    Id = 3,
                    LoteId = loteId,
                    CodigoLote = $"MNA-2026-{loteId:000000}",
                    TipoMovimentacao = TipoMovimentacao.AlteracaoStatus,
                    Descricao = "Status alterado de EmEstoque para EmTransporte",
                    StatusAnterior = "EmEstoque",
                    StatusNovo = "EmTransporte",
                    LocalizacaoAnterior = "Pátio Carajás",
                    LocalizacaoNova = "EFVM - Trem 456",
                    DataHoraMovimentacao = DateTime.Now.AddDays(-5),
                    UsuarioResponsavel = "efvm@vale.com"
                },
                new HistoricoMovimentacao
                {
                    Id = 4,
                    LoteId = loteId,
                    CodigoLote = $"MNA-2026-{loteId:000000}",
                    TipoMovimentacao = TipoMovimentacao.AlteracaoLocalizacao,
                    Descricao = "Movimentado de 'EFVM - Trem 456' para 'Porto Tubarão'",
                    LocalizacaoAnterior = "EFVM - Trem 456",
                    LocalizacaoNova = "Porto Tubarão",
                    DataHoraMovimentacao = DateTime.Now.AddDays(-2),
                    UsuarioResponsavel = "porto@vale.com"
                },
                new HistoricoMovimentacao
                {
                    Id = 5,
                    LoteId = loteId,
                    CodigoLote = $"MNA-2026-{loteId:000000}",
                    TipoMovimentacao = TipoMovimentacao.Embarque,
                    Descricao = "Lote embarcado com destino a China",
                    StatusAnterior = "EmTransporte",
                    StatusNovo = "Embarcado",
                    LocalizacaoAnterior = "Porto Tubarão",
                    LocalizacaoNova = "Navio Vale Beijing",
                    DataHoraMovimentacao = DateTime.Now.AddHours(-12),
                    UsuarioResponsavel = "embarque@vale.com",
                    Observacoes = "Destino: Porto de Qingdao, China"
                }
            };
        }

        private List<HistoricoMovimentacao> ObterTodosHistoricos()
        {
            // Simulação - substituir por consulta real ao banco
            var historicos = new List<HistoricoMovimentacao>();
            for (int i = 1; i <= 3; i++)
            {
                historicos.AddRange(ObterHistoricoSimulado(i));
            }
            return historicos;
        }
    }

    // Classes de Request para os endpoints POST
    public class RegistroStatusRequest
    {
        public int LoteId { get; set; }
        public StatusLote NovoStatus { get; set; }
        public string? Usuario { get; set; }
        public string? Observacoes { get; set; }
    }

    public class RegistroLocalizacaoRequest
    {
        public int LoteId { get; set; }
        public string NovaLocalizacao { get; set; } = "";
        public string? Usuario { get; set; }
        public string? Observacoes { get; set; }
    }

    public class RegistroEmbarqueRequest
    {
        public int LoteId { get; set; }
        public string Destino { get; set; } = "";
        public string? NumeroNavio { get; set; }
        public string? Usuario { get; set; }
    }
}
