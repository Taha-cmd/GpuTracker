namespace GpuTracker.SchemaModels
{
    public class Gpu
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Url { get; set; }
        public string Vendor { get; set; }


        // // https://github.com/ch-robinson/dotnet-avro/tree/main/examples/Chr.Avro.DefaultValuesExample/Models
        // [DefaultValue("")] // for v2 schema
        // public string Wattage { get; set; }
    }
}
