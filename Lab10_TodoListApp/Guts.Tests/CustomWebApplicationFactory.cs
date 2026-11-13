using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoListApp.Domain;
using ToDoListApp.Infrastructure;
using ToDoListApp.Web;

namespace Guts.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection>? _overrideDependencies;
    private SqliteConnection? _connection;

    public CustomWebApplicationFactory(Action<IServiceCollection>? overrideDependencies = null)
    {
        _overrideDependencies = overrideDependencies;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestServer();
        builder.ConfigureTestServices(services =>
        {
            _overrideDependencies?.Invoke(services);

            if (!typeof(DbContext).IsAssignableFrom(typeof(ToDoListContext)))
            {
                return;
            }

            var dbContextOptionsType = typeof(DbContextOptions<>).MakeGenericType(typeof(ToDoListContext));
            var descriptors = services.Where(d =>
                d.ServiceType == dbContextOptionsType ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(ToDoListContext) ||
                (d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("EntityFrameworkCore")) ||
                (d.ImplementationType != null && d.ImplementationType.FullName != null && d.ImplementationType.FullName.Contains("EntityFrameworkCore"))
            ).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == nameof(EntityFrameworkServiceCollectionExtensions.AddDbContext)
                            && m.IsGenericMethodDefinition
                            && m.GetGenericArguments().Length == 1
                            && m.GetParameters().Length == 4);

            var genericMethod = addDbContextMethod.MakeGenericMethod(typeof(ToDoListContext));
            Action<DbContextOptionsBuilder> configureOptions = options => options.UseSqlite(_connection);
            genericMethod.Invoke(null, new object?[] { services, configureOptions, ServiceLifetime.Scoped, ServiceLifetime.Scoped });

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService(typeof(ToDoListContext)) as DbContext;
            dbContext?.Database.EnsureCreated();
        });
    }

    public void SeedData(Action<DbContext> seedAction)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService(typeof(ToDoListContext)) as DbContext;
        if (dbContext is null)
        {
            throw new InvalidOperationException("ToDoListContext must inherit from DbContext.");
        }

        seedAction(dbContext);
        dbContext.SaveChanges();
    }

    public static void SeedSampleLists(DbContext dbContext)
    {
        var groceries = ToDoList.CreateNew("Groceries");
        var milk = ToDoItem.CreateNew("Buy milk");
        var bread = ToDoItem.CreateNew("Buy bread");
        bread.IsDone = true;
        groceries.Items.Add(milk);
        groceries.Items.Add(bread);

        var homework = ToDoList.CreateNew("School Homework");
        var lab = ToDoItem.CreateNew("Finish Lab 10");
        homework.Items.Add(lab);

        dbContext.Set<ToDoList>().AddRange(groceries, homework);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}
