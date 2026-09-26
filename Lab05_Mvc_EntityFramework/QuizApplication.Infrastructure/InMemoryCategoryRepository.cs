using QuizApplication.Domain;

namespace QuizApplication.Infrastructure
{
    internal class InMemoryCategoryRepository
    {
        /* This is an in-memory repository, you do NOT need EF for this. */
        public List<Category> _categories { get; set; }

        public InMemoryCategoryRepository()
        {
            _categories = new List<Category> {
            new Category
            {
                Id = 1,
                Name = "Biology"
            },
            new Category
            {
                Id = 2,
                Name = "Arts and Culture"
            },
            new Category
            {
                Id = 3,
                Name = "Astronomy"
            },
             new Category
            {
                Id = 4,
                Name = "Geography"
            }
         };
        }
    }
}
