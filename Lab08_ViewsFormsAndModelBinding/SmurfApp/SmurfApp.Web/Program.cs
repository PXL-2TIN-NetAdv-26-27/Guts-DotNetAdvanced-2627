using Microsoft.EntityFrameworkCore;
using SmurfApp.AppLogic;
using SmurfApp.Infrastructure;
using System.Diagnostics;

namespace SmurfApp.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<SmurfDbContext>(options =>
        {
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
            options.UseSqlServer(connectionString);
            options.LogTo(message =>
                {
                    Debug.WriteLine(message);
                    Console.WriteLine(message);
                }, 
                [DbLoggerCategory.Database.Command.Name], 
                LogLevel.Information);
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });
        builder.Services.AddScoped<ISmurfStore, SmurfDbStore>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}