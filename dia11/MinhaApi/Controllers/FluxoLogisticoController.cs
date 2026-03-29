using Microsoft.AspNetCore.Mvc;
using MinhaApi.Models;
using MinhaApi.Services;

namespace MinhaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FluxoLogisticoController : ControllerBase
    {
        /// <summary>
        /// POST: api/FluxoLogistico/avancar
        /// Avança o lote para o próximo status no fluxo logístico
        /// </summary>
        [HttpPost("avancar")]
        public ActionResult<object> AvancarStatus([FromBody] AvancarStatusRequest request)
        {
            var lote = BuscarLotePorId(request.LoteId);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {request.LoteId} não encontrado" });

            // Verificar se pode avançar
            if (!GerenciadorFluxoLogistico.PodeAvancar(lote))
            {
                return BadRequest(new 
                { 
                    mensagem = "Lote já está no status final (Embarcado)",
                    statusAtual = lote.Status.ToString()
                });
            }

            var resultado = GerenciadorFluxoLogistico.AvancarStatus(
                lote,
                request.NovaLocalizacao,
                request.Usuario,
                request.Observacoes
            );

            // Aqui você salvaria as alterações no banco
            // _context.SaveChanges();

            return Ok(new
            {
                Sucesso = resultado.Sucesso,
                Mensagem = resultado.Mensagem,
                Lote = new
                {
                    lote.Id,
                    lote.CodigoLote,
                    StatusAnterior = resultado.StatusAnterior.ToString(),
                    StatusNovo = resultado.StatusNovo.ToString(),
                    LocalizacaoAtual = lote.LocalizacaoAtual
                },
                ProximoPasso = resultado.PodeAvancar ? new
                {
                    Status = resultado.ProximoStatusPossivel?.ToString(),
                    Descricao = "Pode continuar avançando"
                } : new
                {
                    Status = (string?)null,
                    Descricao = "Status final alcançado"
                },
                ProgressoFluxo = GerenciadorFluxoLogistico.CalcularProgressoFluxo(lote)
            });
        }

        /// <summary>
        /// POST: api/FluxoLogistico/retroceder
        /// Retrocede o lote para o status anterior (usado em casos de exceção)
        /// </summary>
        [HttpPost("retroceder")]
        public ActionResult<object> RetrocederStatus([FromBody] RetrocederStatusRequest request)
        {
            var lote = BuscarLotePorId(request.LoteId);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {request.LoteId} não encontrado" });

            if (lote.Status == StatusLote.EmEstoque)
            {
                return BadRequest(new 
                { 
                    mensagem = "Lote já está no primeiro status (EmEstoque). Não pode retroceder.",
                    statusAtual = lote.Status.ToString()
                });
            }

            var resultado = GerenciadorFluxoLogistico.RetrocederStatus(
                lote,
                request.NovaLocalizacao,
                request.Usuario,
                request.Motivo
            );

            // Salvar no banco
            // _context.SaveChanges();

            return Ok(new
            {
                Sucesso = resultado.Sucesso,
                Mensagem = resultado.Mensagem,
                Alerta = "⚠️ RETROCESSO REALIZADO",
                Motivo = request.Motivo,
                Lote = new
                {
                    lote.Id,
                    lote.CodigoLote,
                    StatusAnterior = resultado.StatusAnterior.ToString(),
                    StatusNovo = resultado.StatusNovo.ToString(),
                    LocalizacaoAtual = lote.LocalizacaoAtual
                },
                ProgressoFluxo = GerenciadorFluxoLogistico.CalcularProgressoFluxo(lote)
            });
        }

        /// <summary>
        /// GET: api/FluxoLogistico/proximo-status/1
        /// Consulta qual seria o próximo status do lote
        /// </summary>
        [HttpGet("proximo-status/{loteId}")]
        public ActionResult<object> ConsultarProximoStatus(int loteId)
        {
            var lote = BuscarLotePorId(loteId);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {loteId} não encontrado" });

            var statusInfo = GerenciadorFluxoLogistico.ObterProximoStatus(lote);
            var podeAvancar = GerenciadorFluxoLogistico.PodeAvancar(lote);

            return Ok(new
            {
                Lote = new
                {
                    lote.Id,
                    lote.CodigoLote,
                    lote.Status,
                    lote.LocalizacaoAtual
                },
                StatusAtual = statusInfo.StatusAtual.ToString(),
                ProximoStatus = statusInfo.ProximoStatus?.ToString(),
                DescricaoProximoPasso = statusInfo.DescricaoProximoPasso,
                PodeAvancar = podeAvancar,
                LocalizacoesSugeridas = statusInfo.LocalizacoesSugeridas,
                ProgressoAtual = GerenciadorFluxoLogistico.CalcularProgressoFluxo(lote)
            });
        }

        /// <summary>
        /// GET: api/FluxoLogistico/fluxo-completo
        /// Retorna todas as etapas do fluxo logístico
        /// </summary>
        [HttpGet("fluxo-completo")]
        public ActionResult<object> ObterFluxoCompleto()
        {
            var fluxo = GerenciadorFluxoLogistico.ObterFluxoCompleto();

            return Ok(new
            {
                TotalEtapas = fluxo.Count,
                Fluxo = fluxo,
                Descricao = "Fluxo logístico padrão: Estoque → Transporte → Embarcado"
            });
        }

        /// <summary>
        /// GET: api/FluxoLogistico/progresso/1
        /// Mostra o progresso visual do lote no fluxo
        /// </summary>
        [HttpGet("progresso/{loteId}")]
        public ActionResult<object> VisualizarProgresso(int loteId)
        {
            var lote = BuscarLotePorId(loteId);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {loteId} não encontrado" });

            var fluxo = GerenciadorFluxoLogistico.ObterFluxoCompleto();
            var progresso = GerenciadorFluxoLogistico.CalcularProgressoFluxo(lote);

            var etapas = fluxo.Select(e => new
            {
                e.Ordem,
                e.Status,
                e.Nome,
                e.Descricao,
                e.IconeSugerido,
                Completo = (int)e.Status <= (int)lote.Status,
                Atual = e.Status == lote.Status
            });

            return Ok(new
            {
                Lote = new
                {
                    lote.Id,
                    lote.CodigoLote,
                    lote.Status,
                    lote.LocalizacaoAtual
                },
                ProgressoPercentual = progresso,
                ProgressoBarra = $"[{'█' * (int)(progresso / 10)}{'░' * (10 - (int)(progresso / 10))}]",
                Etapas = etapas
            });
        }

        /// <summary>
        /// POST: api/FluxoLogistico/simular-fluxo-completo
        /// Simula o fluxo completo de um lote (útil para testes)
        /// </summary>
        [HttpPost("simular-fluxo-completo")]
        public ActionResult<object> SimularFluxoCompleto([FromBody] SimularFluxoRequest request)
        {
            var lote = BuscarLotePorId(request.LoteId);
            if (lote == null)
                return NotFound(new { mensagem = $"Lote ID {request.LoteId} não encontrado" });

            var resultados = new List<object>();
            var statusInicial = lote.Status;

            // Avançar até o final
            while (GerenciadorFluxoLogistico.PodeAvancar(lote))
            {
                var resultado = GerenciadorFluxoLogistico.AvancarStatus(
                    lote,
                    $"Localização simulada - {lote.Status}",
                    request.Usuario ?? "sistema.simulacao",
                    "Simulação de fluxo completo"
                );

                resultados.Add(new
                {
                    Passo = resultados.Count + 1,
                    StatusAnterior = resultado.StatusAnterior.ToString(),
                    StatusNovo = resultado.StatusNovo.ToString(),
                    Mensagem = resultado.Mensagem,
                    Progresso = GerenciadorFluxoLogistico.CalcularProgressoFluxo(lote)
                });

                // Aguardar simulado (opcional)
                if (request.SimularTempo)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }

            return Ok(new
            {
                Simulacao = "Fluxo completo executado",
                StatusInicial = statusInicial.ToString(),
                StatusFinal = lote.Status.ToString(),
                TotalPassos = resultados.Count,
                Passos = resultados,
                ProgressoFinal = GerenciadorFluxoLogistico.CalcularProgressoFluxo(lote)
            });
        }

        // ===== MÉTODOS AUXILIARES =====

        private LoteMinerio? BuscarLotePorId(int id)
        {
            // Simulação - substituir por busca real no banco
            return new LoteMinerio
            {
                Id = id,
                CodigoLote = $"MNA-2026-{id:000000}",
                MinaOrigem = "Carajás N4E",
                TeorFe = 67.5m,
                Umidade = 7.2m,
                SiO2 = 2.5m,
                Toneladas = 15000,
                Status = StatusLote.EmEstoque,
                LocalizacaoAtual = "Pátio Carajás",
                DataProducao = DateTime.Now.AddDays(-5)
            };
        }
    }

    // Classes de Request
    public class AvancarStatusRequest
    {
        public int LoteId { get; set; }
        public string? NovaLocalizacao { get; set; }
        public string? Usuario { get; set; }
        public string? Observacoes { get; set; }
    }

    public class RetrocederStatusRequest
    {
        public int LoteId { get; set; }
        public string? NovaLocalizacao { get; set; }
        public string? Usuario { get; set; }
        public string Motivo { get; set; } = "";
    }

    public class SimularFluxoRequest
    {
        public int LoteId { get; set; }
        public string? Usuario { get; set; }
        public bool SimularTempo { get; set; } = false;
    }
}
