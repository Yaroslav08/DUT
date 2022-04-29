using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Quiz;
namespace URLS.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class QuizzesController : ApiBaseController
    {
        private readonly IQuizService _quizService;
        private readonly IPermissionService _permissionService;
        public QuizzesController(IQuizService quizService, IPermissionService permissionService)
        {
            _quizService = quizService;
            _permissionService = permissionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] QuizCreateModel quiz)
        {
            return JsonResult(await _quizService.CreateAsync(quiz));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return JsonResult(await _quizService.GetByIdAsync(id, true));
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartQuiz(Guid id)
        {
            return JsonResult(await _quizService.StartQuizAsync(id));
        }

        [HttpPost("{id}/finish/{resulId}")]
        public async Task<IActionResult> FinishQuiz(Guid id, int resultId, QuizAnswerCreateModel model)
        {
            model.QuizId = id;
            return JsonResult(await _quizService.FinishQuizAsync(resultId, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(Guid id)
        {
            return JsonResult(await _quizService.DeleteAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(Guid id, [FromBody] QuizEditModel model)
        {
            model.Id = id;
            return JsonResult(await _quizService.UpdateAsync(model));
        }

        [HttpPut("{id}/questions/{questionId}")]
        public async Task<IActionResult> UpdateQuestion(Guid id, int questionId, [FromBody] QuestionEditModel model)
        {
            model.Id = questionId;
            var list = new List<QuestionEditModel> { model };
            return JsonResult(await _quizService.UpdateQuestionsAsync(id, list));
        }

        [HttpPut("{id}/questions/bulk")]
        public async Task<IActionResult> UpdateQuestionBulk(Guid id, int questionId, [FromBody] List<QuestionEditModel> models)
        {
            return JsonResult(await _quizService.UpdateQuestionsAsync(id, models));
        }

        [HttpDelete("{id}/questions/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(Guid id, int questionId)
        {
            return JsonResult(await _quizService.DeleteQuestionAsync(id, questionId));
        }

        [HttpPut("{id}/questions/{questionId}/answers/bulk")]
        public async Task<IActionResult> UpdateAnswer(Guid id, int questionId, [FromBody] List<AnswerEditModel> models)
        {
            return JsonResult(await _quizService.UpdateAnswersAsync(id, questionId, models));
        }

        [HttpPut("{id}/questions/{questionId}/answers/{answerId}")]
        public async Task<IActionResult> UpdateAnswer(Guid id, int questionId, long answerId, [FromBody] AnswerEditModel model)
        {
            model.Id = answerId;
            var models = new List<AnswerEditModel> { model };
            return JsonResult(await _quizService.UpdateAnswersAsync(id, questionId, models));
        }

        [HttpDelete("{id}/questions/{questionId}/answers/{answerId}")]
        public async Task<IActionResult> DeleteAnswer(Guid id, int questionId, long answerId)
        {
            return JsonResult(await _quizService.DeleteAnswerAsync(id, questionId, answerId));
        }
    }
}