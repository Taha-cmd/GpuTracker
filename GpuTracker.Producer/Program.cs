// https://docs.confluent.io/kafka-clients/dotnet/current/overview.html#ak-dotnet

using System.Text.Json;
using Confluent.Kafka;
using GpuTracker.Producer;

string bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS");
Console.WriteLine("connecting to kafka brokers: " + bootstrapServers);

var config = new ProducerConfig()
{
    BootstrapServers = bootstrapServers,
    AllowAutoCreateTopics = true
};
using var producer = new ProducerBuilder<string, string>(config).Build();

foreach (var gpu in DataGenerator.GetGpus())
{
    var message = new Message<string, string>()
    {
        Key = gpu.Name,
        Value = JsonSerializer.Serialize(gpu)
    };

    Console.WriteLine("sending message: " + message.Value);
    producer.Produce("Gpus", message, report =>
    {
        Console.WriteLine(report.Status + " " + report.Value);
    });
}

producer.Flush();