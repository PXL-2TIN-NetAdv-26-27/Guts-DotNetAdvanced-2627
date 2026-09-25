namespace School.Web.Models;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Credits { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }

    public Course()
    {
        Title = string.Empty;
        Enrollments = new List<Enrollment>();
    }
}