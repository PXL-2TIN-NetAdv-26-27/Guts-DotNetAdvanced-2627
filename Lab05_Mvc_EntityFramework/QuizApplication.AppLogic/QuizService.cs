using QuizApplication.Domain;

namespace QuizApplication.AppLogic
{
    internal class QuizService
    {
        public IReadOnlyList<Category> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public Question GetQuestionByIdWithAnswersAndExtra(int id)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<Question> GetQuestionsInCategory(int id)
        {
            throw new NotImplementedException();
        }
    }
}
