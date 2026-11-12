using BethanysPieShop.Models;
using Guts.Client.XUnit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BethanysPieShop.Tests;

[ExerciseTestClass("dotnet2", "9-RestApi", "OrderDbRepository",
    @"BethanysPieShop\Models\OrderDbRepository.cs;BethanysPieShop\Models\IOrderRepository.cs")]
public class OrderDbRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<BethanysPieShopDbContext> _contextOptions;

    public OrderDbRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new BethanysPieShopDbContext(_contextOptions);
        context.Database.EnsureCreated();
    }

    private BethanysPieShopDbContext CreateDbContext() => new(_contextOptions);

    private static Order CreateSampleOrder(string firstName = "John", string lastName = "Doe", string address = "Elfde-Liniestraat 24")
    {
        return new Order
        {
            FirstName = firstName,
            LastName = lastName,
            AddressLine1 = address,
            ZipCode = "3500",
            City = "Hasselt",
            Country = "Belgium",
            PhoneNumber = "011775555",
            Email = $"{firstName.ToLower()}@pxl.be",
            OrderTotal = 45.00m,
            OrderPlaced = DateTime.UtcNow
        };
    }

    [MonitoredFact]
    public void GetAllOrders_ShouldReturnAllOrdersFromDatabase()
    {
        using (var seedContext = CreateDbContext())
        {
            seedContext.Orders.AddRange(
                CreateSampleOrder("Alice", "Wonderland", "Street 1"),
                CreateSampleOrder("Bob", "Builder", "Street 2"),
                CreateSampleOrder("Charlie", "Chocolate", "Street 3")
            );
            seedContext.SaveChanges();
        }

        using var testContext = CreateDbContext();
        var repository = new OrderDbRepository(testContext, null!);

        var result = repository.GetAllOrders();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, o => o.FirstName == "Alice");
        Assert.Contains(result, o => o.FirstName == "Bob");
        Assert.Contains(result, o => o.FirstName == "Charlie");
    }

    [MonitoredFact]
    public void GetOrderById_ExistingId_ShouldReturnMatchingOrder()
    {
        Order seededOrder = CreateSampleOrder("Dave", "Smith", "Main Street 10");
        using (var seedContext = CreateDbContext())
        {
            seedContext.Orders.Add(seededOrder);
            seedContext.SaveChanges();
        }

        using var testContext = CreateDbContext();
        var repository = new OrderDbRepository(testContext, null!);

        var result = repository.GetOrderById(seededOrder.OrderId);

        Assert.NotNull(result);
        Assert.Equal(seededOrder.OrderId, result.OrderId);
        Assert.Equal("Dave", result.FirstName);
        Assert.Equal("Smith", result.LastName);
    }

    [MonitoredFact]
    public void GetOrderById_NonExistingId_ShouldReturnNull()
    {
        using (var seedContext = CreateDbContext())
        {
            seedContext.Orders.Add(CreateSampleOrder("Eve", "Johnson", "First Avenue 5"));
            seedContext.SaveChanges();
        }

        using var testContext = CreateDbContext();
        var repository = new OrderDbRepository(testContext, null!);

        var result = repository.GetOrderById(99999);

        Assert.Null(result);
    }

    [MonitoredFact]
    public void UpdateOrder_ShouldPersistChangesToDatabase()
    {
        Order seededOrder = CreateSampleOrder("Frank", "Miller", "Original Street 1");
        using (var seedContext = CreateDbContext())
        {
            seedContext.Orders.Add(seededOrder);
            seedContext.SaveChanges();
        }

        using (var updateContext = CreateDbContext())
        {
            var repository = new OrderDbRepository(updateContext, null!);
            var orderToUpdate = repository.GetOrderById(seededOrder.OrderId);
            Assert.NotNull(orderToUpdate);

            orderToUpdate.AddressLine1 = "Updated Street 99";
            typeof(Order).GetProperty("OrderProcessed")?.SetValue(orderToUpdate, true);

            repository.UpdateOrder(orderToUpdate);
        }

        using (var verifyContext = CreateDbContext())
        {
            var persistedOrder = verifyContext.Orders.AsNoTracking().FirstOrDefault(o => o.OrderId == seededOrder.OrderId);
            Assert.NotNull(persistedOrder);
            Assert.Equal("Updated Street 99", persistedOrder.AddressLine1);

            var orderProcessedProp = typeof(Order).GetProperty("OrderProcessed");
            if (orderProcessedProp != null)
            {
                var isProcessed = (bool)orderProcessedProp.GetValue(persistedOrder)!;
                Assert.True(isProcessed, "OrderProcessed should be updated to true in the database.");
            }
        }
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
