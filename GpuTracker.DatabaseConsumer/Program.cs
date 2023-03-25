namespace GpuTracker.DatabaseConsumer
{
    using Chr.Avro.Confluent;
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using GpuTracker.Common;
    using GpuTracker.Database;
    using GpuTracker.GpuModels;
    using GpuTracker.SchemaModels;
    using Newtonsoft.Json;

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


            string sqliteDatabaseConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? throw new Exception("Could not get Environment Variable 'DATABASE_CONNECTION_STRING'");
            var dbContext = new GpuTrackerDatabaseContext(sqliteDatabaseConnectionString);
            IRepository<DbGpu, int> repository = new GpuRepository(dbContext);

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

                        Gpu gpu = cr.Message.Value;
                        repository.Create(GpuMapper.ConvertToDbGpu(gpu));
                        Console.WriteLine($"Wrote GPU (Kafka Key: {key}) to DB: {JsonConvert.SerializeObject(gpu)}");
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


            var allGpus = repository.Get();
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