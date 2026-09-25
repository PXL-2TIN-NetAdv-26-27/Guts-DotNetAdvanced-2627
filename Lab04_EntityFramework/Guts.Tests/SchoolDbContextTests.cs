using System.Reflection;
using Guts.Client.XUnit;
using Microsoft.EntityFrameworkCore;
using School.Web.Infrastructure;
using School.Web.Models;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "4-EF", "School", @"School.Web\Infrastructure\SchoolDbContext.cs")]
public class SchoolDbContextTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public SchoolDbContextTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [MonitoredFact]
    public void ShouldDeriveFromDbContext()
    {
        Assert.True(typeof(DbContext).IsAssignableFrom(typeof(SchoolDbContext)),
            "'SchoolDbContext' should inherit from 'Microsoft.EntityFrameworkCore.DbContext'.");
    }

    [MonitoredFact]
    public void ShouldHaveDbContextOptionsConstructor()
    {
        var constructors = typeof(SchoolDbContext).GetConstructors();
        bool hasOptionsConstructor = constructors.Any(c =>
        {
            var parameters = c.GetParameters();
            return parameters.Length == 1 &&
                   parameters[0].ParameterType.IsGenericType &&
                   parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) &&
                   parameters[0].ParameterType.GetGenericArguments()[0] == typeof(SchoolDbContext);
        });

        Assert.True(hasOptionsConstructor,
            "'SchoolDbContext' should have a public constructor that accepts 'DbContextOptions<SchoolDbContext>'.");
    }

    [MonitoredFact]
    public void ShouldHaveDbSetProperties()
    {
        var dbSetStudent = typeof(SchoolDbContext).GetProperty("Students", BindingFlags.Public | BindingFlags.Instance);
        var dbSetCourse = typeof(SchoolDbContext).GetProperty("Courses", BindingFlags.Public | BindingFlags.Instance);
        var dbSetEnrollment = typeof(SchoolDbContext).GetProperty("Enrollments", BindingFlags.Public | BindingFlags.Instance);

        Assert.True(dbSetStudent != null && dbSetStudent.PropertyType == typeof(DbSet<Student>),
            "'SchoolDbContext' should have a public property 'DbSet<Student> Students'.");
        Assert.True(dbSetCourse != null && dbSetCourse.PropertyType == typeof(DbSet<Course>),
            "'SchoolDbContext' should have a public property 'DbSet<Course> Courses'.");
        Assert.True(dbSetEnrollment != null && dbSetEnrollment.PropertyType == typeof(DbSet<Enrollment>),
            "'SchoolDbContext' should have a public property 'DbSet<Enrollment> Enrollments'.");
    }

    [MonitoredFact]
    public void OnModelCreating_ShouldSeedStudentsCoursesAndEnrollments()
    {
        using var context = _fixture.CreateDbContext();
        Assert.True(context != null,
            "'SchoolDbContext' must inherit from 'DbContext' and have a public constructor accepting 'DbContextOptions<SchoolDbContext>'.");

        var seededStudents = context.Set<Student>().ToList();
        var seededCourses = context.Set<Course>().ToList();
        var seededEnrollments = context.Set<Enrollment>().ToList();

        Assert.True(seededStudents.Count >= 2,
            $"Expected at least 2 seeded students, but found {seededStudents.Count}.");
        Assert.True(seededCourses.Count >= 3,
            $"Expected at least 3 seeded courses, but found {seededCourses.Count}.");
        Assert.True(seededEnrollments.Count >= 6,
            $"Expected at least 6 seeded enrollments, but found {seededEnrollments.Count}.");

        Assert.Contains(seededStudents, s => s.FirstName == "John" && s.LastName == "Doe");
        Assert.Contains(seededStudents, s => s.FirstName == "Jane" && s.LastName == "Doe");
        Assert.Contains(seededCourses, c => c.Title == "Chemistry");
        Assert.Contains(seededCourses, c => c.Title == "Calculus");
        Assert.Contains(seededCourses, c => c.Title == "Literature");
    }

    [MonitoredFact]
    public void OnModelCreating_ShouldConfigureRelationships()
    {
        using var context = _fixture.CreateDbContext();
        Assert.True(context != null,
            "'SchoolDbContext' must inherit from 'DbContext' and have a public constructor accepting 'DbContextOptions<SchoolDbContext>'.");

        var model = context.Model;

        var enrollmentEntity = model.FindEntityType(typeof(Enrollment));
        Assert.True(enrollmentEntity != null, "Could not find 'Enrollment' in the EF model.");

        var studentFk = enrollmentEntity.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Student));
        Assert.True(studentFk != null,
            "No foreign key relationship found between 'Enrollment' and 'Student'.");

        var courseFk = enrollmentEntity.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Course));
        Assert.True(courseFk != null,
            "No foreign key relationship found between 'Enrollment' and 'Course'.");
    }

    [MonitoredFact]
    public void ShouldHaveInitialMigration()
    {
        using var context = _fixture.CreateDbContext();
        Assert.True(context != null,
            "'SchoolDbContext' must inherit from 'DbContext' and have a public constructor accepting 'DbContextOptions<SchoolDbContext>'.");

        var migrations = context.Database.GetMigrations().ToList();

        Assert.True(migrations.Any(m => m.EndsWith("_Initial", StringComparison.OrdinalIgnoreCase) || m.Equals("Initial", StringComparison.OrdinalIgnoreCase)),
            "Could not find a migration named 'Initial' (e.g. 'YYYYMMDDHHMMSS_Initial.cs'). Make sure you added the 'Initial' migration.");
    }

    [MonitoredFact]
    public void ShouldHaveSeedDataMigration()
    {
        using var context = _fixture.CreateDbContext();
        Assert.True(context != null,
            "'SchoolDbContext' must inherit from 'DbContext' and have a public constructor accepting 'DbContextOptions<SchoolDbContext>'.");

        var migrations = context.Database.GetMigrations().ToList();

        Assert.True(migrations.Any(m => m.EndsWith("_SeedData", StringComparison.OrdinalIgnoreCase) || m.Equals("SeedData", StringComparison.OrdinalIgnoreCase)),
            "Could not find a migration named 'SeedData' (e.g. 'YYYYMMDDHHMMSS_SeedData.cs'). Make sure you added the 'SeedData' migration.");
    }
}
