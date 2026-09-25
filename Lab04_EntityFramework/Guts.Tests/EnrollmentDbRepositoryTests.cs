using Guts.Client.XUnit;
using School.Web.Infrastructure;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "4-EF", "School", @"School.Web\Infrastructure\EnrollmentDbRepository.cs")]
public class EnrollmentDbRepositoryTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public EnrollmentDbRepositoryTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [MonitoredFact]
    public void ShouldImplementIEnrollmentRepository()
    {
        Assert.True(typeof(IEnrollmentRepository).IsAssignableFrom(typeof(EnrollmentDbRepository)),
            "'EnrollmentDbRepository' should implement 'IEnrollmentRepository'.");
    }

    [MonitoredFact]
    public void ShouldHaveConstructorAcceptingSchoolDbContext()
    {
        var constructor = typeof(EnrollmentDbRepository).GetConstructor(new[] { typeof(SchoolDbContext) });
        Assert.True(constructor != null,
            "'EnrollmentDbRepository' should have a public constructor that accepts 'SchoolDbContext'.");
    }

    [MonitoredFact]
    public void GetAllGradedEnrollmentsWithStudentAndCourse_ShouldReturnOnlyGradedEnrollments()
    {
        using var context = _fixture.CreateDbContext();
        Assert.True(context != null,
            "'SchoolDbContext' must inherit from 'DbContext' and have a public constructor accepting 'DbContextOptions<SchoolDbContext>'.");

        var repositoryConstructor = typeof(EnrollmentDbRepository).GetConstructor(new[] { typeof(SchoolDbContext) });
        Assert.True(repositoryConstructor != null,
            "'EnrollmentDbRepository' should have a public constructor that accepts 'SchoolDbContext'.");

        var repository = (IEnrollmentRepository)Activator.CreateInstance(typeof(EnrollmentDbRepository), context)!;

        var results = repository.GetAllGradedEnrollmentsWithStudentAndCourse();

        Assert.NotEmpty(results);
        Assert.All(results, enrollment =>
        {
            Assert.True(enrollment.Grade.HasValue,
                $"Expected enrollment '{enrollment.Id}' to have a grade, but Grade was null.");
        });

        // In seeded data, exactly 4 out of 6 enrollments are graded
        Assert.Equal(4, results.Count);
    }

    [MonitoredFact]
    public void GetAllGradedEnrollmentsWithStudentAndCourse_ShouldEagerLoadStudentAndCourse()
    {
        using var context = _fixture.CreateDbContext();
        Assert.True(context != null,
            "'SchoolDbContext' must inherit from 'DbContext' and have a public constructor accepting 'DbContextOptions<SchoolDbContext>'.");

        var repositoryConstructor = typeof(EnrollmentDbRepository).GetConstructor(new[] { typeof(SchoolDbContext) });
        Assert.True(repositoryConstructor != null,
            "'EnrollmentDbRepository' should have a public constructor that accepts 'SchoolDbContext'.");

        var repository = (IEnrollmentRepository)Activator.CreateInstance(typeof(EnrollmentDbRepository), context)!;

        var results = repository.GetAllGradedEnrollmentsWithStudentAndCourse();

        Assert.NotEmpty(results);
        Assert.All(results, enrollment =>
        {
            Assert.True(enrollment.Student != null,
                $"Enrollment '{enrollment.Id}' has a null 'Student' navigation property. Did you use '.Include(e => e.Student)'?");
            Assert.True(enrollment.Course != null,
                $"Enrollment '{enrollment.Id}' has a null 'Course' navigation property. Did you use '.Include(e => e.Course)'?");
        });
    }

    [MonitoredFact]
    public void GetAllGradedEnrollmentsWithStudentAndCourse_ShouldSortByStudentLastNameThenFirstName()
    {
        using var context = _fixture.CreateDbContext();
        Assert.True(context != null,
            "'SchoolDbContext' must inherit from 'DbContext' and have a public constructor accepting 'DbContextOptions<SchoolDbContext>'.");

        var repositoryConstructor = typeof(EnrollmentDbRepository).GetConstructor(new[] { typeof(SchoolDbContext) });
        Assert.True(repositoryConstructor != null,
            "'EnrollmentDbRepository' should have a public constructor that accepts 'SchoolDbContext'.");

        var repository = (IEnrollmentRepository)Activator.CreateInstance(typeof(EnrollmentDbRepository), context)!;

        var results = repository.GetAllGradedEnrollmentsWithStudentAndCourse().ToList();

        Assert.True(results.Count >= 2, "Expected at least 2 graded enrollments to verify sorting.");

        for (int i = 0; i < results.Count - 1; i++)
        {
            var current = results[i].Student!;
            var next = results[i + 1].Student!;

            int lastNameComparison = string.Compare(current.LastName, next.LastName, StringComparison.OrdinalIgnoreCase);
            if (lastNameComparison == 0)
            {
                int firstNameComparison = string.Compare(current.FirstName, next.FirstName, StringComparison.OrdinalIgnoreCase);
                Assert.True(firstNameComparison <= 0,
                    $"Enrollments are not sorted correctly by student FirstName ascending. Found '{current.FirstName}' before '{next.FirstName}'.");
            }
            else
            {
                Assert.True(lastNameComparison < 0,
                    $"Enrollments are not sorted correctly by student LastName ascending. Found '{current.LastName}' before '{next.LastName}'.");
            }
        }
    }
}
