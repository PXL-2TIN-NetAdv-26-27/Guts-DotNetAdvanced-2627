using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BethanysPieShop.Models;

namespace BethanysPieShop.Tests;

public class CustomWebApplicationFactory(Action<IServiceCollection>? overrideDependencies = null)
    : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestServer();
        builder.ConfigureTestServices(services =>
        {
            overrideDependencies?.Invoke(services);

            // Remove all existing DbContext and EF provider registrations
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<BethanysPieShopDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(BethanysPieShopDbContext) ||
                (d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("EntityFrameworkCore")) ||
                (d.ImplementationType != null && d.ImplementationType.FullName != null && d.ImplementationType.FullName.Contains("EntityFrameworkCore"))
            ).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Create and open SQLite in-memory connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<BethanysPieShopDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BethanysPieShopDbContext>();
            dbContext.Database.EnsureCreated();

            if (!dbContext.Orders.Any())
            {
                var sampleOrder = new Order
                {
                    FirstName = "John",
                    LastName = "Doe",
                    AddressLine1 = "Elfde-Liniestraat 24",
                    ZipCode = "3500",
                    City = "Hasselt",
                    Country = "Belgium",
                    PhoneNumber = "011775555",
                    Email = "john.doe@pxl.be",
                    OrderTotal = 35.90m,
                    OrderPlaced = DateTime.UtcNow
                };

                // Set OrderProcessed to false if the property exists
                typeof(Order).GetProperty("OrderProcessed")?.SetValue(sampleOrder, false);

                dbContext.Orders.Add(sampleOrder);
                dbContext.SaveChanges();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
