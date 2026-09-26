using Microsoft.AspNetCore.Mvc;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using QuizApplication.Web.Models;
using System.Collections.Generic;

namespace QuizApplication.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly ILogger<QuizController> _logger;

        public QuizController(ILogger<QuizController> logger, IQuizService quizService)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult QuestionsInCategory(int id)
        {
            return View();
        }

        public IActionResult QuestionWithAnswers(int id)
        {
            return View();
        }
    }
}
