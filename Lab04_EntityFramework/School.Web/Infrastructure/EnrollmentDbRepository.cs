using School.Web.Models;

namespace School.Web.Infrastructure;

public class EnrollmentDbRepository : IEnrollmentRepository
{
    public IReadOnlyCollection<Enrollment> GetAllGradedEnrollmentsWithStudentAndCourse()
    {
        throw new NotImplementedException();
    }
}