using Guts.Client.Core.TestTools;
using Guts.Client.XUnit;
using Xunit;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "3-MVC", "Tutorial")]
public class TutorialTests
{
    [MonitoredFact]
    public void _01_MvcMovie_Web_Folder_ShouldContainMvcProject()
    {
        string projectPath = Path.Combine(Solution.Current.Path, "MvcMovie.Web", "MvcMovie.Web.csproj");

        Assert.True(File.Exists(projectPath), $"Cannot find project '{projectPath}'");
    }

    [MonitoredFact]
    public void _02_HelloWorldController_ShouldHaveWelcomeAction()
    {
        string path = "MvcMovie.Web/Controllers/HelloWorldController.cs";
        string fileContent = string.Empty;
        try
        {
            fileContent = Solution.Current.GetFileContent(path);
        }
        catch
        {
            Assert.Fail($"Cannot find controller '{path}'");
        }

        Assert.True(fileContent.Contains("Welcome("), "Cannot find a 'Welcome' action");
    }

    [MonitoredFact]
    public void _03_HelloWorld_Index_View_ShouldBePresent()
    {
        string path = Path.Combine(Solution.Current.Path, "MvcMovie.Web", "Views", "HelloWorld", "Index.cshtml");

        Assert.True(File.Exists(path), $"Cannot find view '{path}'");
    }

    [MonitoredFact]
    public void _04_Movie_Model_ShouldBePresent()
    {
        string path = Path.Combine(Solution.Current.Path, "MvcMovie.Web", "Models", "Movie.cs");

        Assert.True(File.Exists(path), $"Cannot find model '{path}'");
    }

    [MonitoredFact]
    public void _05_MoviesController_ShouldHaveCrudActions()
    {
        string path = "MvcMovie.Web/Controllers/MoviesController.cs";
        string fileContent = string.Empty;
        try
        {
            fileContent = Solution.Current.GetFileContent(path);
        }
        catch
        {
            Assert.Fail($"Cannot find controller '{path}'");
        }

        Assert.True(fileContent.Contains("Index("), "Cannot find a 'Index' action");
        Assert.True(fileContent.Contains("Details("), "Cannot find a 'Details' action");
        Assert.True(fileContent.Contains("Create("), "Cannot find a 'Create' action");
        Assert.True(fileContent.Contains("Edit("), "Cannot find a 'Edit' action");
        Assert.True(fileContent.Contains("Delete("), "Cannot find a 'Delete' action");
    }

    [MonitoredFact]
    public void _06_ShouldHaveA_MvcMovieDbContext()
    {
        string path = Path.Combine(Solution.Current.Path, "MvcMovie.Web", "Data", "MvcMovieContext.cs");

        Assert.True(File.Exists(path), $"Cannot find DbContext '{path}'");
    }

    [MonitoredFact]
    public void _07_ShouldHaveA_InitialCreate_Migration()
    {
        string migrationsDir = Path.Combine(Solution.Current.Path, "MvcMovie.Web", "Migrations");

        Assert.True(Directory.Exists(migrationsDir), "Cannot find 'MvcMovie.Web/Migrations' folder");

        string[] migrationFiles = Directory.GetFiles(migrationsDir, "*.cs", SearchOption.AllDirectories);
        Assert.True(migrationFiles.Length > 0, "No migration files found in 'MvcMovie.Web/Migrations'");

        bool hasInitialCreate = migrationFiles.Any(f => f.Contains("InitialCreate"));
        Assert.True(hasInitialCreate, "No migration file containing 'InitialCreate' was found");
    }

    [MonitoredFact]
    public void _08_Movies_Index_Create_Details_Views_ShouldBePresent()
    {
        //Index
        string path = Path.Combine(Solution.Current.Path, "MvcMovie.Web", "Views", "Movies", "Index.cshtml");
        Assert.True(File.Exists(path), $"Cannot find view '{path}'");

        //Create
        path = Path.Combine(Solution.Current.Path, "MvcMovie.Web", "Views", "Movies", "Create.cshtml");
        Assert.True(File.Exists(path), $"Cannot find view '{path}'");

        //Details
        path = Path.Combine(Solution.Current.Path, "MvcMovie.Web", "Views", "Movies", "Details.cshtml");
        Assert.True(File.Exists(path), $"Cannot find view '{path}'");
    }

    [MonitoredFact]
    public void _09_SeedDataClass_ShouldBePresent()
    {
        //Create
        string path = Path.Combine(Solution.Current.Path, "MvcMovie.Web", "Models", "SeedData.cs");
        Assert.True(File.Exists(path), $"Cannot find class '{path}'");
    }
}