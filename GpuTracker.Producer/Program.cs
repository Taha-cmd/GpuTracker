// https://docs.confluent.io/kafka-clients/dotnet/current/overview.html#ak-dotnet

// https://www.youtube.com/watch?v=6O4LJKEqlF8
// https://github.com/confluentinc/confluent-kafka-dotnet

using Chr.Avro.Confluent; // https://engineering.chrobinson.com/dotnet-avro/guides/kafka/
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using GpuTracker.Common;
using GpuTracker.Models;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Crosscutting;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.State;
using Streamiz.Kafka.Net.Table;

namespace GpuTracker.Producer
{
    public class Program
    {
        const string TOPIC_NAME = "Gpus";
        const string TOPIC_AGGREGATED_PRICES = "gpu-average-price";

        public static void Main()
        {
            ProduceGpus();

            StreamAveragePrice();

            ConsumeAveragePrice();
        }

        private static async void ProduceGpus()
        {
            string schemaRegistryUrl = Environment.GetEnvironmentVariable("SCHEMA_REGISTRY_URL");
            Console.WriteLine("connecting schema registry: " + schemaRegistryUrl);
            var schemaRegistryConfig = new SchemaRegistryConfig()
            {
                Url = schemaRegistryUrl
            };

            string bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS");
            Console.WriteLine("connecting to kafka brokers: " + bootstrapServers);

            var config = new ProducerConfig()
            {
                BootstrapServers = bootstrapServers,
                AllowAutoCreateTopics = true,
            };

            using var producer = new ProducerBuilder<string, Gpu>(config)
                .SetAvroValueSerializer(schemaRegistryConfig, AutomaticRegistrationBehavior.Always)
                .Build();

            foreach (var gpu in DataGenerator.GetGpus())
            {
                var message = new Message<string, Gpu>()
                {
                    Key = gpu.Name,
                    Value = gpu
                };

                Console.WriteLine("sending message: " + message.Value);
                var result = await producer.ProduceAsync(TOPIC_NAME, message);
                Console.WriteLine(result.Status + " " + result.Message);
            }

            producer.Flush();
        }

        private static async void StreamAveragePrice()
        {
            string bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS");
            string schemaRegistryUrl = Environment.GetEnvironmentVariable("SCHEMA_REGISTRY_URL");
            var schemaRegistryConfig = new SchemaRegistryConfig()
            {
                Url = schemaRegistryUrl
            };

            // streaming stuff
            var streamConfig = new StreamConfig()
            {
                BootstrapServers = bootstrapServers,
                SchemaRegistryUrl = schemaRegistryUrl,
                AllowAutoCreateTopics = true,
                AutoRegisterSchemas = true,
                ApplicationId = "gpu-tracker",
                DefaultValueSerDes = new AvroSerDes<Gpu>(schemaRegistryConfig),
                DefaultKeySerDes = new StringSerDes(),
                CommitIntervalMs = 100,
                Guarantee = ProcessingGuarantee.AT_LEAST_ONCE
            };

            // https://lgouellec.github.io/kafka-streams-dotnet/statefull-processors.html#aggregate
            var streamBuilder = new StreamBuilder();
            var ktable = streamBuilder.Stream<string, Gpu>(TOPIC_NAME)
                .GroupBy((_, gpu) => gpu.Name)
                .Aggregate(() => 0.0d,
                    (key, gpu, aggregator) => aggregator + gpu.Price,
                    Materialized<string, double, IKeyValueStore<Bytes, byte[]>>
                        .Create("store").WithValueSerdes(new DoubleSerDes()).WithKeySerdes(new StringSerDes())).MapValues(d => d);

            // RocksDbNative nuget package must be installed
            ktable.ToStream().To(TOPIC_AGGREGATED_PRICES, new StringSerDes(), new DoubleSerDes());
            var topology = streamBuilder.Build();
            var streams = new KafkaStream(topology, streamConfig);
            await streams.StartAsync();
        }

        private static void ConsumeAveragePrice()
        {
            string bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS");
            var config = new ProducerConfig()
            {
                BootstrapServers = bootstrapServers,
                AllowAutoCreateTopics = true
            };

            var consumerConfig = new ConsumerConfig(config);
            consumerConfig.GroupId = "test-group";

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, byte[]>(consumerConfig).Build())
            {
                consumer.Subscribe(TOPIC_AGGREGATED_PRICES);
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        string key = cr.Message.Key == null ? "Null" : cr.Message.Key;
                        double doubleValue = BitConverter.ToDouble(cr.Message.Value);

                        Console.WriteLine($"Consumed record with key {key} and value (double) {doubleValue}");
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
    }
}