using School.Web.Models;

namespace School.Web.Infrastructure;

/// <summary>
/// Returns all enrollments that have a non-null grade.
/// For each enrollment, the student and course relation is loaded (not null).
/// </summary>
public interface IEnrollmentRepository
{
    IReadOnlyCollection<Enrollment> GetAllGradedEnrollmentsWithStudentAndCourse();
}