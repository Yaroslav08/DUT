using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Quiz;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class QuizService : BaseService<Quiz>, IQuizService
    {
        private readonly URLSDbContext _db;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        public QuizService(URLSDbContext db, IIdentityService identityService, IMapper mapper) : base(db)
        {
            _db = db;
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<Result<QuizViewModel>> GetQuizByIdAsync(Guid id, bool withQuestions = false)
        {
            var quiz = await _db.Quizzes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (quiz == null)
                return Result<QuizViewModel>.NotFound(typeof(Quiz).NotFoundMessage(id));

            var quizViewModel = _mapper.Map<QuizViewModel>(quiz);

            if (withQuestions)
                quizViewModel.Questions = _mapper.Map<List<QuestionViewModel>>(await _db.Questions.AsNoTracking().Where(s => s.QuizId == id).OrderBy(s => s.Index).ToListAsync());

            return Result<QuizViewModel>.SuccessWithData(quizViewModel);
        }

        public async Task<Result<List<QuizViewModel>>> GetQuizzesBySubjectIdAsync(int subjectId, int offset = 0, int count = 10)
        {
            var quizzes = await _db.Quizzes
                .AsNoTracking()
                .Where(s => s.SubjectId == subjectId)
                .OrderByDescending(s => s.ActiveFrom)
                .Skip(offset).Take(count)
                .ToListAsync();
            var quizzesViewModel = _mapper.Map<List<QuizViewModel>>(quizzes);
            return Result<List<QuizViewModel>>.SuccessWithData(quizzesViewModel);
        }

        public async Task<Result<List<QuizResultViewModel>>> GetResultsAsync(Guid quizId, int offset = 0, int count = 10)
        {
            var results = await _db.QuizResults
                .AsNoTracking()
                .Where(s => s.QuizId == quizId)
                .OrderByDescending(s => s.CreatedAt)
                .Skip(offset).Take(count)
                .ToListAsync();
            var resultsViewModel = _mapper.Map<List<QuizResultViewModel>>(results);
            return Result<List<QuizResultViewModel>>.SuccessWithData(resultsViewModel);
        }

        public async Task<Result<List<QuizResultViewModel>>> GetUserResultsAsync(int userId, int offset = 0, int count = 10)
        {
            var results = await _db.QuizResults
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .Skip(offset).Take(count)
                .ToListAsync();
            var resultsViewModel = _mapper.Map<List<QuizResultViewModel>>(results);
            return Result<List<QuizResultViewModel>>.SuccessWithData(resultsViewModel);
        }
    }
}