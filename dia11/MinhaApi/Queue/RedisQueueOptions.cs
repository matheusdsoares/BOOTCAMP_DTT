namespace MinhaApi.Queue
{
    public class RedisQueueOptions
    {
        public string StreamName { get; set; } = "lotes:processamento";
        public string ConsumerGroup { get; set; } = "workers-lote";
        public string ConsumerName { get; set; } = "worker-1";
        public int MaxDeliveries { get; set; } = 5; // máximo de reentregas antes de DLQ
        public string DeadLetterStream { get; set; } = "lotes:processamento:dead";
    }
}