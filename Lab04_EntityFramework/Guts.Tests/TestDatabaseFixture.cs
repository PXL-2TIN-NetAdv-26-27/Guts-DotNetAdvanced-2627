using System.Data.Common;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using School.Web.Infrastructure;

namespace Guts.Tests;

public class TestDatabaseFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDatabaseFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public DbContext? CreateDbContext()
    {
        if (!typeof(DbContext).IsAssignableFrom(typeof(SchoolDbContext)))
        {
            return null;
        }

        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(typeof(SchoolDbContext));
        var optionsBuilder = Activator.CreateInstance(optionsBuilderType);

        var useSqliteMethod = typeof(SqliteDbContextOptionsBuilderExtensions)
            .GetMethods()
            .First(m => m.Name == "UseSqlite"
                        && m.GetParameters().Length >= 2
                        && typeof(DbConnection).IsAssignableFrom(m.GetParameters()[1].ParameterType));

        useSqliteMethod.Invoke(null, new object?[] { optionsBuilder, _connection, null });

        var options = optionsBuilderType.GetProperty("Options", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!.GetValue(optionsBuilder);

        var constructor = typeof(SchoolDbContext).GetConstructor(new[] { options!.GetType() })
            ?? typeof(SchoolDbContext).GetConstructor(new[] { typeof(DbContextOptions) })
            ?? typeof(SchoolDbContext).GetConstructor(Type.EmptyTypes);

        if (constructor == null) return null;

        var parameters = constructor.GetParameters().Length == 0 ? Array.Empty<object>() : new[] { options };
        var context = Activator.CreateInstance(typeof(SchoolDbContext), parameters) as DbContext;
        context?.Database.EnsureCreated();
        return context;
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
