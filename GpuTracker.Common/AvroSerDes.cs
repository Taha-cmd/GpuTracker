// https://docs.confluent.io/kafka-clients/dotnet/current/overview.html#ak-dotnet

// https://www.youtube.com/watch?v=6O4LJKEqlF8
// https://github.com/confluentinc/confluent-kafka-dotnet

using Chr.Avro.Confluent; // https://engineering.chrobinson.com/dotnet-avro/guides/kafka/
using Confluent.Kafka;
using Streamiz.Kafka.Net.SerDes;
// using this implementation (is used by the producer)
// https://github.com/ch-robinson/dotnet-avro/blob/main/src/Chr.Avro.Confluent/Confluent/AsyncSchemaRegistryDeserializer.cs

namespace GpuTracker.Common
{
    public class AvroSerDes<T> : ISerDes<T>
    {
        private readonly IAsyncDeserializer<T> deserializer;
        private readonly IAsyncSerializer<T> serializer;
        
        public AvroSerDes(IEnumerable<KeyValuePair<string, string>> schemaRegistryConfig)
        {
            this.deserializer = new AsyncSchemaRegistryDeserializer<T>(schemaRegistryConfig);
            this.serializer = new AsyncSchemaRegistrySerializer<T>(schemaRegistryConfig, AutomaticRegistrationBehavior.Always);
        }
        public T Deserialize(byte[] data, SerializationContext context)
        {
            var result = Task.Run<T>(() => this.deserializer.DeserializeAsync(data, data == null, context)).Result;
            return result;
        }

        public object DeserializeObject(byte[] data, SerializationContext context)
        {
            var result = Task.Run<T>(() => this.deserializer.DeserializeAsync(data, data == null, context)).Result;
            return result;
        }

        public void Initialize(SerDesContext context)
        {
        }

        public byte[] Serialize(T data, SerializationContext context)
        {
            var result = Task.Run<byte[]>(() => this.serializer.SerializeAsync(data, context)).Result;
            return result;
        }

        public byte[] SerializeObject(object data, SerializationContext context)
        {
            if (data is T gpu)
            {
                var result = Task.Run<byte[]>(() => this.serializer.SerializeAsync(gpu, context)).Result;
                return result;
            }
            else
            {
                throw new Exception("Data to serialize is not of Type " + typeof(T));
            }
        }
    }
}