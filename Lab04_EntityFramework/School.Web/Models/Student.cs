namespace School.Web.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public DateTime StartedOn { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }

        public Student()
        {
            Enrollments = new List<Enrollment>();
        }        
    }
}
