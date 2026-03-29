using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace MinhaApi.Queue
{
    public class LoteQueueWorker : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly RedisQueueOptions _opts;
        private readonly ILogger<LoteQueueWorker> _logger;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public LoteQueueWorker(
            IConnectionMultiplexer redis,
            IOptions<RedisQueueOptions> opts,
            ILogger<LoteQueueWorker> logger)
        {
            _redis = redis;
            _opts = opts.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _redis.GetDatabase();
            var stream = _opts.StreamName;
            var group = _opts.ConsumerGroup;
            var consumer = _opts.ConsumerName;

            // Cria o Consumer Group se não existir
            try
            {
                await db.StreamCreateConsumerGroupAsync(stream, group, "$");
                _logger.LogInformation("Consumer Group {Group} criado no stream {Stream}.", group, stream);
            }
            catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
            {
                _logger.LogInformation("Consumer Group {Group} já existe.", group);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Primeiro, reprocessa mensagens pendentes (delivery falho) deste consumer
                    await ReentregarPendentesAsync(db, stream, group, stoppingToken);

                    // Lê novas mensagens
                    var results = await db.StreamReadGroupAsync(stream, group, consumer, ">", count: 10);

                    if (results.Length == 0) continue;

                    foreach (var msg in results)
                    {
                        try
                        {
                            var payloadValue = msg.Values.FirstOrDefault(v => v.Name == "payload").Value;
                            if (payloadValue.IsNullOrEmpty)
                            {
                                _logger.LogWarning("Mensagem {Id} sem payload.", msg.Id);
                                await db.StreamAcknowledgeAsync(stream, group, msg.Id);
                                continue;
                            }

                            var json = payloadValue.ToString(); // 👈 força tipo string
                            var evento = JsonSerializer.Deserialize<ProcessarLoteMessage>(json, _json)!;
                            await ProcessarAsync(evento, stoppingToken);
                            await db.StreamAcknowledgeAsync(stream, group, msg.Id);

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro processando mensagem {Id}", msg.Id);
                            // Não dá ACK aqui; a mensagem fica pendente e será reentregue
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no loop do worker de fila.");
                    await Task.Delay(2000, stoppingToken);
                }
            }
        }

        private async Task ReentregarPendentesAsync(IDatabase db, string stream, string group, CancellationToken ct)
        {
            // Verifica mensagens pendentes para este group
            var info = await db.StreamPendingAsync(stream, group);
            if (info.PendingMessageCount == 0) return;

            // Busca em lotes pequenos
            var pending = await db.StreamPendingMessagesAsync(stream, group, 20, _opts.ConsumerName);

            foreach (var p in pending)
            {
                // Simplesmente TENTE claim — o Redis vai filtrar por IdleTime automaticamente
                var claimed = await db.StreamClaimAsync(
                    stream,
                    group,
                    _opts.ConsumerName,
                    minIdleTimeInMs: 15000,
                    messageIds: new[] { p.MessageId }
                );

                if (claimed.Length == 0)
                    continue; // não tinha IdleTime >= 15s

                var entry = claimed[0];

                if (p.DeliveryCount >= _opts.MaxDeliveries)
                {
                    await MoverParaDeadLetterAsync(db, entry, ct);
                    await db.StreamAcknowledgeAsync(stream, group, entry.Id);
                    continue;
                }

                // Depois o XREADGROUP vai pegá-las novamente
            }
        }

        private async Task MoverParaDeadLetterAsync(IDatabase db, StreamEntry entry, CancellationToken ct)
        {
            var dlq = _opts.DeadLetterStream;
            await db.StreamAddAsync(dlq, entry.Values);
            _logger.LogWarning("Mensagem {Id} movida para DLQ {DLQ}.", entry.Id, dlq);
        }

        private async Task ProcessarAsync(ProcessarLoteMessage msg, CancellationToken ct)
        {
            // 👉 Aqui entra a lógica de negócio "assíncrona".
            // Exemplo: recalcular classificação e gravar um histórico/auditoria.
            // (Você pode injetar DbContext via escopo, se preferir, criando IServiceScopeFactory)

            // Regrinha didática de classificação só como exemplo:
            var classificacao =
                msg.TeorFe >= 65 && msg.Umidade <= 6 ? "Premium" :
                msg.TeorFe >= 62 ? "Padrão" :
                "Baixa";

            // (Exemplo) Simular IO/integração externa
            await Task.Delay(200, ct);

            // Log
            Console.WriteLine($"[Fila] Lote {msg.CodigoLote} (Id={msg.LoteId}) -> Ação={msg.Acao} | Classificação={classificacao}");
        }
    }
}