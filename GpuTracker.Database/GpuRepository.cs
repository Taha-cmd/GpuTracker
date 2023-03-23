using GpuTracker.GpuModels;
using System.Security.Cryptography.X509Certificates;

namespace GpuTracker.Database
{
    public class GpuRepository : IRepository<DbGpu, int>
    {
        public IGpuTrackerDbContext dbContext { get; set; }

        public GpuRepository(IGpuTrackerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DbGpu Create(DbGpu element)
        {
            dbContext.Gpu.Add(element);
            dbContext.SaveChanges();

            return element;
        }

        public void Delete(int id)
        {
            var gpu = dbContext.Gpu.Find(id);
            dbContext.Gpu.Remove(gpu);
            this.dbContext.SaveChanges();
        }

        public DbGpu Get(int id)
        {
            var gpu = dbContext.Gpu.Find(id);
            return gpu;
        }

        public List<DbGpu> Get()
        {
            var list = this.dbContext.Gpu.ToList();

            return list;
        }

        public void Update(DbGpu element)
        {
            this.dbContext.Gpu.Update(element);
            this.dbContext.SaveChanges();
        }

        public List<DbGpu> GetByQuery(string s)
        {
            var list = this.dbContext.Gpu
                       .Where(x => 
                            x.Name.ToUpper().Contains(s.ToUpper()) ||
                            x.Vendor.ToUpper().Contains(s.ToUpper()) || 
                            x.Url.ToUpper().Contains(s.ToUpper()) ||
                            x.Price.ToString().ToUpper().Contains(s.ToUpper()))
                       .ToList();

            return list;
        }
    }
}
