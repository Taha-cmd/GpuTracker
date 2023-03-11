// https://docs.confluent.io/kafka-clients/dotnet/current/overview.html#ak-dotnet

// https://www.youtube.com/watch?v=6O4LJKEqlF8
// https://github.com/confluentinc/confluent-kafka-dotnet

using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Chr.Avro.Confluent; // https://engineering.chrobinson.com/dotnet-avro/guides/kafka/
using GpuTracker.Producer;
using GpuTracker.Producer.Models;

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

using var producer = new ProducerBuilder<Null, Gpu>(config)
    .SetAvroValueSerializer(schemaRegistry, AutomaticRegistrationBehavior.Always)
    .Build();

foreach (var gpu in DataGenerator.GetGpus())
{
    var message = new Message<Null, Gpu>()
    {
        Value = gpu
    };

    Console.WriteLine("sending message: " + message.Value);
    var result = await producer.ProduceAsync("Gpus", message);
    Console.WriteLine(result.Status + " " + result.Message);
}

producer.Flush();