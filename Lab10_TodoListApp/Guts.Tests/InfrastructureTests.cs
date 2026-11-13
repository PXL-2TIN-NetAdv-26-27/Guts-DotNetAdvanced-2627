using System.Reflection;
using Guts.Client.XUnit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.AppLogic;
using ToDoListApp.Domain;
using ToDoListApp.Infrastructure;

namespace Guts.Tests;

[ExerciseTestClass("dotnet2", "10-ToDoListApp", "Infrastructure",
    @"ToDoListApp.Infrastructure\ToDoListContext.cs")]
public class InfrastructureTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public InfrastructureTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    private DbContext CreateContext()
    {
        Assert.True(typeof(DbContext).IsAssignableFrom(typeof(ToDoListContext)),
            "ToDoListContext must inherit from DbContext.");

        var constructors = typeof(ToDoListContext).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        Assert.Single(constructors);

        var parameters = constructors[0].GetParameters();
        Assert.Single(parameters);
        Assert.True(typeof(DbContextOptions).IsAssignableFrom(parameters[0].ParameterType),
            "ToDoListContext constructor must accept DbContextOptions.");

        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(typeof(ToDoListContext));
        var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilderType)!;
        optionsBuilder.UseSqlite(_connection);

        var context = (DbContext)Activator.CreateInstance(typeof(ToDoListContext), optionsBuilder.Options)!;
        context.Database.EnsureCreated();
        return context;
    }

    private static IToDoListRepository CreateListRepository(DbContext context)
    {
        var repoType = typeof(ToDoListContext).Assembly.GetTypes()
            .FirstOrDefault(t => typeof(IToDoListRepository).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        Assert.NotNull(repoType);
        return (IToDoListRepository)Activator.CreateInstance(repoType!, context)!;
    }

    private static IToDoItemRepository CreateItemRepository(DbContext context)
    {
        var repoType = typeof(ToDoListContext).Assembly.GetTypes()
            .FirstOrDefault(t => typeof(IToDoItemRepository).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        Assert.NotNull(repoType);
        return (IToDoItemRepository)Activator.CreateInstance(repoType!, context)!;
    }

    [MonitoredFact]
    public void _01_ToDoListContext_ShouldHaveOnlyOneDbSetForToDoLists_AndSingleDbContextOptionsConstructor()
    {
        using var context = CreateContext();

        var dbSetProperties = typeof(ToDoListContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .ToList();

        Assert.Single(dbSetProperties);
        Assert.Equal(typeof(DbSet<ToDoList>), dbSetProperties[0].PropertyType);
    }

    [MonitoredFact]
    public void _02_ToDoListRepository_AddNewAndGetById_ShouldPersistAndReturnListWithItems()
    {
        using (var setupContext = CreateContext())
        {
            var repo = CreateListRepository(setupContext);
            ToDoList created = repo.AddNew("Shopping");
            repo.AddItemToExistingList(created.Id, "Buy apples");
        }

        using var verifyContext = CreateContext();
        var verifyRepo = CreateListRepository(verifyContext);
        var lists = verifyRepo.Find(null);
        Assert.Single(lists);

        ToDoList? loaded = verifyRepo.GetById(lists[0].Id);
        Assert.NotNull(loaded);
        Assert.Equal("Shopping", loaded!.Title);
        Assert.Single(loaded.Items);
        Assert.Equal("Buy apples", loaded.Items[0].Description);
    }

    [MonitoredFact]
    public void _03_ToDoListRepository_GetById_NonExisting_ShouldReturnNull()
    {
        using var context = CreateContext();
        var repo = CreateListRepository(context);

        ToDoList? result = repo.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [MonitoredFact]
    public void _04_ToDoListRepository_Find_ShouldFilterCaseInsensitivelyAndOrderByTitle()
    {
        using (var setupContext = CreateContext())
        {
            var repo = CreateListRepository(setupContext);
            repo.AddNew("Work Tasks");
            repo.AddNew("Groceries");
            repo.AddNew("Weekend Work");
        }

        using var verifyContext = CreateContext();
        var verifyRepo = CreateListRepository(verifyContext);

        IList<ToDoList> allLists = verifyRepo.Find(null);
        Assert.Equal(3, allLists.Count);
        Assert.Equal("Groceries", allLists[0].Title);
        Assert.Equal("Weekend Work", allLists[1].Title);
        Assert.Equal("Work Tasks", allLists[2].Title);

        IList<ToDoList> filtered = verifyRepo.Find("work");
        Assert.Equal(2, filtered.Count);
        Assert.Equal("Weekend Work", filtered[0].Title);
        Assert.Equal("Work Tasks", filtered[1].Title);
    }

    [MonitoredFact]
    public void _05_ToDoItemRepository_GetTotalOfItemsThatAreNotDone_ShouldCountOnlyUndoneItems()
    {
        using (var setupContext = CreateContext())
        {
            var listRepo = CreateListRepository(setupContext);
            ToDoList list = listRepo.AddNew("Daily");
            listRepo.AddItemToExistingList(list.Id, "Task 1");
            listRepo.AddItemToExistingList(list.Id, "Task 2");
            listRepo.AddItemToExistingList(list.Id, "Task 3");

            ToDoList? loaded = listRepo.GetById(list.Id);
            var itemRepo = CreateItemRepository(setupContext);
            itemRepo.Update(loaded!.Items[0].Id, true);
        }

        using var verifyContext = CreateContext();
        var verifyItemRepo = CreateItemRepository(verifyContext);

        int count = verifyItemRepo.GetTotalOfItemsThatAreNotDone();

        Assert.Equal(2, count);
    }

    [MonitoredFact]
    public void _06_ToDoItemRepository_Update_ShouldPersistIsDoneChange()
    {
        Guid itemId;
        using (var setupContext = CreateContext())
        {
            var listRepo = CreateListRepository(setupContext);
            ToDoList list = listRepo.AddNew("Errands");
            listRepo.AddItemToExistingList(list.Id, "Post package");
            itemId = listRepo.GetById(list.Id)!.Items[0].Id;
        }

        using (var updateContext = CreateContext())
        {
            var itemRepo = CreateItemRepository(updateContext);
            ToDoItem updated = itemRepo.Update(itemId, true);
            Assert.True(updated.IsDone);
        }

        using var verifyContext = CreateContext();
        var item = verifyContext.Set<ToDoItem>().Find(itemId);
        Assert.NotNull(item);
        Assert.True(item!.IsDone);
    }
}
