using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;

namespace QuizApplication.Infrastructure
{
    internal class QuestionRepository : IQuestionRepository
    {
        public QuestionRepository(QuizDbContext dbContext)
        {

        }

        public IReadOnlyList<Question> GetByCategoryId(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Question GetByIdWithAnswers(int id)
        {
            throw new NotImplementedException();
        }
    }
}
