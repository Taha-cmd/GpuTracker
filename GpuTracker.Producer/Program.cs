// https://docs.confluent.io/kafka-clients/dotnet/current/overview.html#ak-dotnet

// https://www.youtube.com/watch?v=6O4LJKEqlF8
// https://github.com/confluentinc/confluent-kafka-dotnet

using Chr.Avro.Confluent; // https://engineering.chrobinson.com/dotnet-avro/guides/kafka/
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using GpuTracker.Producer;
using GpuTracker.Producer.Models;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Crosscutting;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.State;
using Streamiz.Kafka.Net.Table;

public class Program
{
    public static async Task Main()
    {
        const string TOPIC_NAME = "Gpus";
        const string TOPIC_AGGREGATED_PRICES = "gpu-average-price";

        string schemaRegistryUrl = Environment.GetEnvironmentVariable("SCHEMA_REGISTRY_URL");
        Console.WriteLine("connecting schema registry: " + schemaRegistryUrl);
        var schemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = schemaRegistryUrl
        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

        string bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS");
        Console.WriteLine("connecting to kafka brokers: " + bootstrapServers);

        var config = new ProducerConfig()
        {
            BootstrapServers = bootstrapServers,
            AllowAutoCreateTopics = true
        };

        using var producer = new ProducerBuilder<string, Gpu>(config)
            .SetAvroValueSerializer(schemaRegistry, AutomaticRegistrationBehavior.Always)
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

        var schemaRegistryForStream = new CachedSchemaRegistryClient(schemaRegistryConfig);

        // streaming stuff
        var streamConfig = new StreamConfig()
        {
            BootstrapServers = bootstrapServers,
            SchemaRegistryUrl = schemaRegistryUrl,
            AllowAutoCreateTopics = true,
            AutoRegisterSchemas = true,
            ApplicationId = "gpu-tracker",
            DefaultValueSerDes = new Implementation(schemaRegistryForStream),
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
}


// using this implementation (is used by the producer)
// https://github.com/ch-robinson/dotnet-avro/blob/main/src/Chr.Avro.Confluent/Confluent/AsyncSchemaRegistryDeserializer.cs
class Implementation : ISerDes<Gpu>
{
    private readonly IAsyncDeserializer<Gpu> deserializer;
    private readonly IAsyncSerializer<Gpu> serializer;
    public Implementation(ISchemaRegistryClient schemaRegistryClient)
    {
        this.deserializer = new AsyncSchemaRegistryDeserializer<Gpu>(schemaRegistryClient);
        this.serializer = new AsyncSchemaRegistrySerializer<Gpu>(schemaRegistryClient);
    }
    public Gpu Deserialize(byte[] data, SerializationContext context)
    {
        var result = Task.Run<Gpu>(() => this.deserializer.DeserializeAsync(data, data == null, context)).Result;
        return result;
    }

    public object DeserializeObject(byte[] data, SerializationContext context)
    {
        var result = Task.Run<Gpu>(() => this.deserializer.DeserializeAsync(data, data == null, context)).Result;
        return result;
    }

    public void Initialize(SerDesContext context)
    {
    }

    public byte[] Serialize(Gpu data, SerializationContext context)
    {
        var result = Task.Run<byte[]>(() => this.serializer.SerializeAsync(data, context)).Result;
        return result;
    }

    public byte[] SerializeObject(object data, SerializationContext context)
    {
        if(data is Gpu gpu)
        {
            var result = Task.Run<byte[]>(() => this.serializer.SerializeAsync(gpu, context)).Result;
            return result;
        }
        else
        {
            throw new Exception("Data to serialize is not of Type " + typeof(Gpu));
        }
    }
}