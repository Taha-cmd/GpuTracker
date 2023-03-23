using Microsoft.EntityFrameworkCore;
using GpuTracker.GpuModels;

namespace GpuTracker.Database
{
    public interface IGpuTrackerDbContext
    {
        DbSet<DbGpu> Gpu { get; set; }

        int SaveChanges();
    }
}
