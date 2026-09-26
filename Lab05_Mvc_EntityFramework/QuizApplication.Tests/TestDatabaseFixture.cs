using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using QuizApplication.Infrastructure;

namespace QuizApplication.Tests;

public class TestDatabaseFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDatabaseFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    internal QuizDbContext CreateDbContext(bool ensureCreated = true)
    {
        var options = new DbContextOptionsBuilder<QuizDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new QuizDbContext(options);
        if (ensureCreated)
        {
            context.Database.EnsureCreated();
        }
        return context;
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
