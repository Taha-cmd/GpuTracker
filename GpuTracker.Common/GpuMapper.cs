using GpuTracker.SchemaModels;
using GpuTracker.GpuModels;

namespace GpuTracker.Common
{
    public class GpuMapper
    {
        public static DbGpu ConvertToDbGpu(Gpu gpu)
        {
            return new DbGpu()
            {
                Name = gpu.Name,
                Price = gpu.Price,
                Url = gpu.Url,
                Vendor = gpu.Vendor,
            };
        }
        public static Gpu ConvertToSchemaGpu(DbGpu gpu)
        {
            return new Gpu()
            {
                Name = gpu.Name,
                Price = gpu.Price,
                Url = gpu.Url,
                Vendor = gpu.Vendor,
            };
        }
    }
}
