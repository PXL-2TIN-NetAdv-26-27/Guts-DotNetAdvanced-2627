using School.Web.Models;

namespace School.Web.Infrastructure;

public class SchoolDbContext
{
    //You can use the private methods below to seed the database with dummy data
    private IList<Course> GetCourses()
    {
        return new List<Course>
        {
            new Course { Id = new Guid("a8d850e4-6ba2-4cd4-bd19-429cf6daf510"), Title = "Chemistry", Credits = 3 },
            new Course { Id = new Guid("63096885-ffdc-400f-98aa-ec3119808447"), Title = "Calculus", Credits = 4 },
            new Course { Id = new Guid("2fe3e4e9-d65b-4880-b86e-bad7592e803e"), Title = "Literature", Credits = 4 }
        };
    }

    private IList<Student> GetDummyStudents()
    {
        return new List<Student>
        {
            new Student
            {
                Id = new Guid("64265584-bccd-4b1a-ba1f-36584a945019"), StartedOn = new DateTime(2023, 9, 1), FirstName = "John", LastName = "Doe"
            },
            new Student
            {
                Id = new Guid("9bd0a453-40d8-4e2c-9e58-7a7a9b6027fd"), StartedOn = new DateTime(2023, 8, 15), FirstName = "Jane", LastName = "Doe"
            }
        };
    }

    private IList<Enrollment> GetDummyEnrollments(IList<Student> students, IList<Course> courses)
    {
        return new List<Enrollment>
        {
            new Enrollment
            {
                Id = new Guid("044adf17-48a3-4d9c-89f2-7a3133b1c0dc"),
                StudentId = students[0].Id,
                CourseId = courses[0].Id,
                Grade = Grade.B
            },
            new Enrollment
            {
                Id = new Guid("193397ba-a21c-4c59-a5da-3576d92eb7ab"),
                StudentId = students[0].Id,
                CourseId = courses[1].Id,
                Grade = null
            },
            new Enrollment
            {
                Id =new Guid("844baa7e-a647-48fc-845d-49c49aaf0bfd"),
                StudentId = students[0].Id,
                CourseId = courses[2].Id,
                Grade = Grade.F
            },
            new Enrollment
            {
                Id =new Guid("a17953b1-493f-4b16-8669-53ffc4749308"),
                StudentId = students[1].Id,
                CourseId = courses[0].Id,
                Grade = null
            },
            new Enrollment
            {
                Id = new Guid("c38311e9-938e-4b06-ab6f-e3f6ddee93cb"),
                StudentId = students[1].Id,
                CourseId = courses[1].Id,
                Grade = Grade.D
            },
            new Enrollment
            {
                Id = new Guid("e5c86faf-129e-45fe-a29f-58b8450765df"),
                StudentId = students[1].Id,
                CourseId = courses[2].Id,
                Grade = Grade.A
            },
        };
    }
}