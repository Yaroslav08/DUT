using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Quiz;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces
{
    public interface IQuizService : IBaseService<Quiz>
    {
        Task<Result<List<QuizViewModel>>> GetBySubjectIdAsync(int subjectId, int offset = 0, int count = 10);
        Task<Result<QuizViewModel>> GetByIdAsync(Guid id, bool withQuestions = false);
        Task<Result<List<QuizResultViewModel>>> GetResultsAsync(Guid quizId, int offset = 0, int count = 10);
        Task<Result<List<QuizResultViewModel>>> GetUserResultsAsync(int userId, int offset = 0, int count = 10);
        Task<Result<QuizViewModel>> CreateAsync(QuizCreateModel quiz);
    }
}
