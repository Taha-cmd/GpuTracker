using GpuTracker.Database;
using GpuTracker.GpuModels;
using Microsoft.AspNetCore.ResponseCompression;

namespace GpuTracker.Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // todo: replace datasource with app setting.
            string sqliteDatabaseConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? throw new Exception("Could not get Environment Variable 'DATABASE_CONNECTION_STRING'");
            builder.Services.AddSingleton<IGpuTrackerDbContext>((m) => new GpuTrackerDatabaseContext(sqliteDatabaseConnectionString));
            builder.Services.AddSingleton<IRepository<DbGpu, int>, GpuRepository>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();


            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}