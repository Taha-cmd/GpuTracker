using GpuTracker.Models;
using Microsoft.EntityFrameworkCore;
namespace GpuTracker.Database
{
    public class GpuRepository : IRepository<Gpu, int>
    {
        public IGpuTrackerDbContext dbContext { get; set; }

        public GpuRepository(IGpuTrackerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Gpu Create(Gpu element)
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

        public Gpu Get(int id)
        {
            var gpu = dbContext.Gpu.Find(id);
            return gpu;
        }

        public List<Gpu> Get()
        {
            var list = this.dbContext.Gpu.ToList();

            return list;
        }

        public void Update(Gpu element)
        {
            this.dbContext.Gpu.Update(element);
            this.dbContext.SaveChanges();
        }
    }
}
