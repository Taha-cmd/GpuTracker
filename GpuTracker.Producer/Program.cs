// https://docs.confluent.io/kafka-clients/dotnet/current/overview.html#ak-dotnet

using Confluent.Kafka;

var config = new ProducerConfig()
{
    BootstrapServers = "localhost:9094"
};

using var producer = new ProducerBuilder<string, string>(config).Build();

for (int i = 10; i < 100; i++)
{
    var message = new Message<string, string>()
    {
        Key = "key-test" + i,
        Value = "leitner" + i
    };

    producer.Produce("Gpus", message, report =>
    {
        Console.WriteLine(report.Status);
    });
}

producer.Flush();