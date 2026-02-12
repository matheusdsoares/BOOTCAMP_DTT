namespace MinhaApi.Services
{
    using MinhaApi.Models;

    public static class GerenciadorFluxoLogistico
    {
        /// <summary>
        /// Avança o status do lote para o próximo estágio do fluxo logístico
        /// Fluxo: EmEstoque → EmTransporte → Embarcado
        /// </summary>
        public static ResultadoAvanco AvancarStatus(
            LoteMinerio lote,
            string? novaLocalizacao = null,
            string? usuario = null,
            string? observacoes = null)
        {
            var statusAnterior = lote.Status;
            var localizacaoAnterior = lote.LocalizacaoAtual;

            switch (lote.Status)
            {
                case StatusLote.EmEstoque:
                    // Estoque → Transporte
                    lote.Status = StatusLote.EmTransporte;
                    lote.LocalizacaoAtual = novaLocalizacao ?? "Em Trânsito";
                    
                    // Registrar no histórico
                    GerenciadorHistorico.RegistrarMudancaStatus(
                        lote, statusAnterior, lote.Status, usuario, observacoes);
                    
                    GerenciadorHistorico.RegistrarMudancaLocalizacao(
                        lote, localizacaoAnterior, lote.LocalizacaoAtual, usuario, 
                        "Lote saiu do estoque para transporte");

                    return new ResultadoAvanco
                    {
                        Sucesso = true,
                        StatusAnterior = statusAnterior,
                        StatusNovo = lote.Status,
                        Mensagem = "Lote movido de Estoque para Transporte",
                        ProximoStatusPossivel = StatusLote.Embarcado,
                        PodeAvancar = true
                    };

                case StatusLote.EmTransporte:
                    // Transporte → Embarcado
                    lote.Status = StatusLote.Embarcado;
                    lote.LocalizacaoAtual = novaLocalizacao ?? "Navio";
                    
                    // Registrar no histórico
                    GerenciadorHistorico.RegistrarMudancaStatus(
                        lote, statusAnterior, lote.Status, usuario, observacoes);
                    
                    GerenciadorHistorico.RegistrarEmbarque(
                        lote, novaLocalizacao ?? "Destino não especificado", 
                        novaLocalizacao, usuario);

                    return new ResultadoAvanco
                    {
                        Sucesso = true,
                        StatusAnterior = statusAnterior,
                        StatusNovo = lote.Status,
                        Mensagem = "Lote embarcado com sucesso",
                        ProximoStatusPossivel = null,
                        PodeAvancar = false
                    };

                case StatusLote.Embarcado:
                    // Já está no status final
                    return new ResultadoAvanco
                    {
                        Sucesso = false,
                        StatusAnterior = statusAnterior,
                        StatusNovo = lote.Status,
                        Mensagem = "Lote já está embarcado. Este é o status final.",
                        ProximoStatusPossivel = null,
                        PodeAvancar = false
                    };

                default:
                    return new ResultadoAvanco
                    {
                        Sucesso = false,
                        StatusAnterior = statusAnterior,
                        StatusNovo = lote.Status,
                        Mensagem = "Status não reconhecido",
                        ProximoStatusPossivel = null,
                        PodeAvancar = false
                    };
            }
        }

        /// <summary>
        /// Retrocede o status do lote (reverter transporte, desembarcar)
        /// </summary>
        public static ResultadoAvanco RetrocederStatus(
            LoteMinerio lote,
            string? novaLocalizacao = null,
            string? usuario = null,
            string? motivo = null)
        {
            var statusAnterior = lote.Status;
            var localizacaoAnterior = lote.LocalizacaoAtual;

            switch (lote.Status)
            {
                case StatusLote.EmEstoque:
                    return new ResultadoAvanco
                    {
                        Sucesso = false,
                        StatusAnterior = statusAnterior,
                        StatusNovo = lote.Status,
                        Mensagem = "Lote já está em Estoque. Não pode retroceder.",
                        ProximoStatusPossivel = null,
                        PodeAvancar = false
                    };

                case StatusLote.EmTransporte:
                    // Transporte → Estoque (devolução)
                    lote.Status = StatusLote.EmEstoque;
                    lote.LocalizacaoAtual = novaLocalizacao ?? "Pátio de Estoque";
                    
                    GerenciadorHistorico.RegistrarMudancaStatus(
                        lote, statusAnterior, lote.Status, usuario, 
                        $"RETROCESSO: {motivo ?? "Motivo não informado"}");

                    return new ResultadoAvanco
                    {
                        Sucesso = true,
                        StatusAnterior = statusAnterior,
                        StatusNovo = lote.Status,
                        Mensagem = "Lote retornado ao Estoque",
                        ProximoStatusPossivel = StatusLote.EmTransporte,
                        PodeAvancar = true
                    };

                case StatusLote.Embarcado:
                    // Embarcado → Transporte (desembarque)
                    lote.Status = StatusLote.EmTransporte;
                    lote.LocalizacaoAtual = novaLocalizacao ?? "Porto";
                    
                    GerenciadorHistorico.RegistrarMudancaStatus(
                        lote, statusAnterior, lote.Status, usuario, 
                        $"DESEMBARQUE: {motivo ?? "Motivo não informado"}");

                    return new ResultadoAvanco
                    {
                        Sucesso = true,
                        StatusAnterior = statusAnterior,
                        StatusNovo = lote.Status,
                        Mensagem = "Lote desembarcado",
                        ProximoStatusPossivel = StatusLote.Embarcado,
                        PodeAvancar = true
                    };

                default:
                    return new ResultadoAvanco
                    {
                        Sucesso = false,
                        StatusAnterior = statusAnterior,
                        StatusNovo = lote.Status,
                        Mensagem = "Status não reconhecido",
                        ProximoStatusPossivel = null,
                        PodeAvancar = false
                    };
            }
        }

        /// <summary>
        /// Verifica se o lote pode avançar para o próximo status
        /// </summary>
        public static bool PodeAvancar(LoteMinerio lote)
        {
            return lote.Status != StatusLote.Embarcado;
        }

        /// <summary>
        /// Retorna informações sobre o próximo status possível
        /// </summary>
        public static StatusInfo ObterProximoStatus(LoteMinerio lote)
        {
            return lote.Status switch
            {
                StatusLote.EmEstoque => new StatusInfo
                {
                    StatusAtual = StatusLote.EmEstoque,
                    ProximoStatus = StatusLote.EmTransporte,
                    DescricaoProximoPasso = "Iniciar transporte do lote",
                    LocalizacoesSugeridas = new List<string> 
                    { 
                        "EFVM - Trem", 
                        "Caminhão de Transporte", 
                        "Ferrovia"
                    }
                },
                StatusLote.EmTransporte => new StatusInfo
                {
                    StatusAtual = StatusLote.EmTransporte,
                    ProximoStatus = StatusLote.Embarcado,
                    DescricaoProximoPasso = "Embarcar o lote",
                    LocalizacoesSugeridas = new List<string> 
                    { 
                        "Navio Vale Beijing", 
                        "Porto Tubarão", 
                        "Navio Cargueiro"
                    }
                },
                StatusLote.Embarcado => new StatusInfo
                {
                    StatusAtual = StatusLote.Embarcado,
                    ProximoStatus = null,
                    DescricaoProximoPasso = "Status final alcançado",
                    LocalizacoesSugeridas = new List<string>()
                },
                _ => new StatusInfo
                {
                    StatusAtual = lote.Status,
                    ProximoStatus = null,
                    DescricaoProximoPasso = "Status desconhecido",
                    LocalizacoesSugeridas = new List<string>()
                }
            };
        }

        /// <summary>
        /// Retorna o fluxo completo esperado para o lote
        /// </summary>
        public static List<EtapaFluxo> ObterFluxoCompleto()
        {
            return new List<EtapaFluxo>
            {
                new EtapaFluxo
                {
                    Ordem = 1,
                    Status = StatusLote.EmEstoque,
                    Nome = "Estoque",
                    Descricao = "Lote armazenado aguardando transporte",
                    IconeSugerido = "📦"
                },
                new EtapaFluxo
                {
                    Ordem = 2,
                    Status = StatusLote.EmTransporte,
                    Nome = "Transporte",
                    Descricao = "Lote em trânsito para o porto",
                    IconeSugerido = "🚂"
                },
                new EtapaFluxo
                {
                    Ordem = 3,
                    Status = StatusLote.Embarcado,
                    Nome = "Embarcado",
                    Descricao = "Lote embarcado no navio",
                    IconeSugerido = "🚢"
                }
            };
        }

        /// <summary>
        /// Retorna o progresso percentual do lote no fluxo logístico
        /// </summary>
        public static decimal CalcularProgressoFluxo(LoteMinerio lote)
        {
            return lote.Status switch
            {
                StatusLote.EmEstoque => 33.33m,
                StatusLote.EmTransporte => 66.67m,
                StatusLote.Embarcado => 100.00m,
                _ => 0m
            };
        }
    }

    // Classes auxiliares
    public class ResultadoAvanco
    {
        public bool Sucesso { get; set; }
        public StatusLote StatusAnterior { get; set; }
        public StatusLote StatusNovo { get; set; }
        public string Mensagem { get; set; } = "";
        public StatusLote? ProximoStatusPossivel { get; set; }
        public bool PodeAvancar { get; set; }
    }

    public class StatusInfo
    {
        public StatusLote StatusAtual { get; set; }
        public StatusLote? ProximoStatus { get; set; }
        public string DescricaoProximoPasso { get; set; } = "";
        public List<string> LocalizacoesSugeridas { get; set; } = new();
    }

    public class EtapaFluxo
    {
        public int Ordem { get; set; }
        public StatusLote Status { get; set; }
        public string Nome { get; set; } = "";
        public string Descricao { get; set; } = "";
        public string IconeSugerido { get; set; } = "";
    }
}