using Microsoft.EntityFrameworkCore;
using GpuTracker.GpuModels;

namespace GpuTracker.Database
{
    public class GpuTrackerDatabaseContext : DbContext, IGpuTrackerDbContext
    {
        private string SqliteConnectionString { get; set; }

        public GpuTrackerDatabaseContext(string sqliteConnectionString)
        {
            this.SqliteConnectionString = sqliteConnectionString;
            this.Database.EnsureCreated();
        }

        public GpuTrackerDatabaseContext(DbContextOptions<GpuTrackerDatabaseContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (!string.IsNullOrEmpty(SqliteConnectionString))
                {
                    optionsBuilder.UseSqlite(SqliteConnectionString);
                }
            }

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<DbGpu> Gpu { get; set; } = default!;
    }
}
