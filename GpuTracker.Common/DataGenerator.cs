using Bogus;
using GpuTracker.Models;

namespace GpuTracker.Common
{
    public class DataGenerator
    {
        public static IReadOnlyCollection<Gpu> GetGpus()
        {
            var faker = new Faker<Gpu>()
                .RuleFor(gpu => gpu.Name, faker => faker.PickRandom("Rtx 3060", "Rtx 3070", "Rtx 3080"))
                .RuleFor(gpu => gpu.Vendor, faker => faker.PickRandom("Media Markt", "Amazon"))
                .RuleFor(gpu => gpu.Price, faker => faker.Random.Double(500, 2000))
                .RuleFor(gpu => gpu.Url, (_, gpu) => $"https://{gpu.Vendor.Replace(" ", "")}.com/{gpu.Name}");
            //.RuleFor(gpu => gpu.Wattage, faker => faker.PickRandom("65W", "85W", "100W"));

            return faker.GenerateBetween(10, 25);
        }
    }
}

