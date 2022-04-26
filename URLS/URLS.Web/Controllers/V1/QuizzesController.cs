using Microsoft.AspNetCore.Http;
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
    }
}
