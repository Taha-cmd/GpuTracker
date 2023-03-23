using GpuTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpuTracker.Database
{
    public interface IGpuTrackerDbContext
    {
        DbSet<Gpu> Gpu { get; set; }

        int SaveChanges();
    }
}
