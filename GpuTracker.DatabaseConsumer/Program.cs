namespace GpuTracker.DatabaseConsumer
{
    using Chr.Avro.Confluent;
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using GpuTracker.Models;

    public class Program
    {
        const string TOPIC_NAME = "Gpus";

        static void Main(string[] args)
        {
            ConsumeGpus();
        }

        private static void ConsumeGpus()
        {
            string bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS") ?? throw new Exception("Could not get Envrionment Variable 'BOOTSTRAP_SERVERS'");
            var config = new ConsumerConfig()
            {
                BootstrapServers = bootstrapServers,
                AllowAutoCreateTopics = true,                
            };

            var consumerConfig = new ConsumerConfig(config);
            consumerConfig.GroupId = "database-consumer";

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, Gpu>(consumerConfig)
                .SetAvroValueDeserializer(getSchemaRegistryConfig())
                .Build())
            {
                consumer.Subscribe(TOPIC_NAME);
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        string key = cr.Message.Key == null ? "Null" : cr.Message.Key;
                        // double doubleValue = BitConverter.ToDouble(cr.Message.Value);

                        Console.WriteLine($"Consumed record with key {key} and value (double) {cr.Message.Value}");
                        // byte[] bytes = Encoding.UTF8.GetBytes(cr.Message.Value);

                    }
                }
                catch (OperationCanceledException)
                {
                    //exception might have occurred since Ctrl-C was pressed.
                }
                finally
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();
                }
            }
        }

        private static SchemaRegistryConfig getSchemaRegistryConfig()
        {
            string schemaRegistryUrl = Environment.GetEnvironmentVariable("SCHEMA_REGISTRY_URL") ?? throw new Exception("Could not get Envrionment Variable 'SCHEMA_REGISTRY_URL'");
            var schemaRegistryConfig = new SchemaRegistryConfig()
            {
                Url = schemaRegistryUrl,
            };

            return schemaRegistryConfig;
        }
    }
}