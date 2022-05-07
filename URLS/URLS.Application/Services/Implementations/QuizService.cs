using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.Validations;
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
        private readonly ISubjectService _subjectService;
        private readonly ICommonService _commonService;
        public QuizService(URLSDbContext db, IIdentityService identityService, IMapper mapper, ISubjectService subjectService, ICommonService commonService) : base(db)
        {
            _db = db;
            _identityService = identityService;
            _mapper = mapper;
            _subjectService = subjectService;
            _commonService = commonService;
        }

        public async Task<Result<QuizViewModel>> CreateAsync(QuizCreateModel quiz)
        {
            if (!quiz.IsTemplate)
                if (!await _subjectService.IsExistAsync(s => s.Id == quiz.SubjectId))
                    return Result<QuizViewModel>.NotFound(typeof(Subject).NotFoundMessage(quiz.SubjectId));

            if (!QuizValidation.TryValidate(quiz, out var error))
                return Result<QuizViewModel>.Error(error);

            var newQuiz = QuizValidation.BuildNewQuiz(quiz, _identityService);
            await _db.Quizzes.AddAsync(newQuiz);
            await _db.SaveChangesAsync();

            var viewModel = _mapper.Map<QuizViewModel>(newQuiz);

            return Result<QuizViewModel>.SuccessWithData(viewModel);
        }

        public async Task<Result<QuizResultViewModel>> FinishQuizAsync(int quizResultId, QuizAnswerCreateModel model)
        {
            var quizResult = await _db.QuizResults.AsNoTracking().Include(s => s.Quiz).FirstOrDefaultAsync(s => s.Id == quizResultId);
            if (quizResult == null)
                return Result<QuizResultViewModel>.NotFound(typeof(QuizResult).NotFoundMessage(quizResultId));

            if (quizResult.UserId != _identityService.GetUserId())
                return Result<QuizResultViewModel>.Forbiden();

            if (quizResult.EndAt != null)
                return Result<QuizResultViewModel>.Error("Test already pass");

            var maxCountOfAttempts = quizResult.Quiz.Config.MaxAttempts;

            var currentCountOfUserAttempts = await _db
                .QuizResults
                .CountAsync(s => s.QuizId == quizResult.QuizId && s.UserId == quizResult.UserId);

            if (maxCountOfAttempts >= 0 && currentCountOfUserAttempts >= maxCountOfAttempts)
                return Result<QuizResultViewModel>.Error("Max attempt of test");
            else
                quizResult.Attempt = currentCountOfUserAttempts + 1;

            var quiz = quizResult.Quiz;
            quizResult.EndAt = DateTime.Now;

            if (quiz.Config.Minutes <= 0)
            {
                quizResult.TimeIsExpired = false;
            }
            else
            {
                var time = DateTime.Now - quizResult.StartAt;
                if (time.TotalMinutes <= quiz.Config.Minutes)
                {
                    quizResult.TimeIsExpired = false;
                }
                else
                {
                    quizResult.TimeIsExpired = true;
                }
            }

            quizResult.Result = new List<QuestionModel>();

            var questions = await _db.Questions.AsNoTracking().Where(s => s.QuizId == model.QuizId).Include(s => s.Answers).ToListAsync();

            if (!TryMapUserAnswersToQuiz(new MapAnswersToQuiz(questions, model.Responses, quizResult, quiz), out var error))//questions, model.Responses, quizResullt, quiz.Config.ShowCorrectAnswers, out var error))
            {
                return Result<QuizResultViewModel>.Error(error);
            }

            quizResult.PrepareToUpdate(_identityService);

            _db.QuizResults.Update(quizResult);
            await _db.SaveChangesAsync();

            var quizResultViewModel = _mapper.Map<QuizResultViewModel>(quizResult);

            if (!quizResult.Quiz.Config.ShowResults)
                quizResultViewModel.Result = null;

            return Result<QuizResultViewModel>.SuccessWithData(quizResultViewModel);
        }

        public async Task<Result<QuizViewModel>> GetByIdAsync(Guid id, bool fullTest = false)
        {
            var quiz = await _db.Quizzes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (quiz == null)
                return Result<QuizViewModel>.NotFound(typeof(Quiz).NotFoundMessage(id));

            if (!CanViewQuiz(quiz))
                return Result<QuizViewModel>.NotFound(typeof(Quiz).NotFoundMessage(id));

            var quizViewModel = _mapper.Map<QuizViewModel>(quiz);

            if (fullTest)
            {
                var questions = await _db.Questions.AsNoTracking().Where(s => s.QuizId == id).Include(s => s.Answers).OrderBy(s => s.Index).ToListAsync();
                if (quiz.Config.RandomQuestionsAndAnswers)
                    quizViewModel.Questions = _mapper.Map<List<QuestionViewModel>>(GetRandomQuestions(questions));
                else
                    quizViewModel.Questions = _mapper.Map<List<QuestionViewModel>>(GetSortingQuestions(questions));
            }

            return Result<QuizViewModel>.SuccessWithData(quizViewModel);
        }

        public async Task<Result<List<QuizViewModel>>> GetBySubjectIdAsync(int subjectId, int offset = 0, int count = 10)
        {
            var quizzes = await _db.Quizzes
                .AsNoTracking()
                .Where(s => s.SubjectId == subjectId)
                .Skip(offset).Take(count)
                .ToListAsync();
            var quizzesViewModel = _mapper.Map<List<QuizViewModel>>(quizzes);
            return Result<List<QuizViewModel>>.SuccessWithData(quizzesViewModel);
        }

        public async Task<Result<List<QuizResultViewModel>>> GetResultsAsync(Guid quizId, int offset = 0, int count = 10)
        {
            var request = await _commonService.IsExistWithResultsAsync<Quiz>(s => s.Id == quizId);

            if (!request.IsExist)
                return Result<List<QuizResultViewModel>>.NotFound(typeof(Quiz).NotFoundMessage(quizId));

            var quiz = request.Results.First();

            var query = _db.QuizResults.AsNoTracking();

            var allResults = quiz.CreatedByUserId == _identityService.GetUserId() || _identityService.IsAdministrator();

            if (allResults)
                query = query.Where(s => s.QuizId == quizId);
            else
                query = query.Where(s => s.QuizId == quizId && s.CreatedByUserId == _identityService.GetUserId());


            query = query.Include(s => s.User)
                .OrderByDescending(s => s.EndAt)
                .Skip(offset).Take(count);

            var results = await query.ToListAsync();
            var resultsViewModel = _mapper.Map<List<QuizResultViewModel>>(results);

            resultsViewModel.ForEach(result =>
            {
                result.Result = null;
            });

            return Result<List<QuizResultViewModel>>.SuccessWithData(resultsViewModel);
        }

        public async Task<Result<List<QuizResultViewModel>>> GetUserResultsAsync(int userId, int offset = 0, int count = 10)
        {
            var results = await _db.QuizResults
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Include(s => s.Quiz)
                .OrderByDescending(s => s.CreatedAt)
                .Skip(offset).Take(count)
                .ToListAsync();
            var resultsViewModel = _mapper.Map<List<QuizResultViewModel>>(results);
            return Result<List<QuizResultViewModel>>.SuccessWithData(resultsViewModel);
        }

        public async Task<Result<QuizStartedViewModel>> StartQuizAsync(Guid quizId)
        {
            if (!await IsExistAsync(s => s.Id == quizId))
                return Result<QuizStartedViewModel>.NotFound(typeof(Quiz).NotFoundMessage(quizId));

            var currentQuiz = Exists.First();

            var currentUserId = _identityService.GetUserId();

            var newResult = new QuizResult
            {
                Mark = 0,
                UserId = currentUserId,
                QuizId = quizId,
                Result = null,
                Statistics = null,
                StartAt = DateTime.Now,
                EndAt = null,
                TimeIsExpired = false
            };

            var maxAttempts = currentQuiz.Config.MaxAttempts;

            if (maxAttempts >= 1)
            {
                var previewAttempts = await _db.QuizResults.CountAsync(s => s.UserId == currentUserId && s.QuizId == quizId);
                if (previewAttempts >= maxAttempts)
                    return Result<QuizStartedViewModel>.Error("Your max count of attempts were expired");
                newResult.Attempt = previewAttempts + 1;
            }

            newResult.PrepareToCreate(_identityService);
            await _db.QuizResults.AddAsync(newResult);
            await _db.SaveChangesAsync();

            var quizViewModel = await ReadyQuizToViewAsync(currentQuiz);

            var resultViewModel = new QuizStartedViewModel
            {
                Result = _mapper.Map<QuizResultViewModel>(newResult),
                Quiz = quizViewModel
            };

            return Result<QuizStartedViewModel>.SuccessWithData(resultViewModel);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var quizToDelete = await _db.Quizzes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (quizToDelete == null)
                return Result<bool>.NotFound(typeof(Quiz).NotFoundMessage(id));

            if (!_identityService.IsAdministrator())
                if (quizToDelete.CreatedByUserId != _identityService.GetUserId())
                    return Result<bool>.Forbiden();

            _db.Quizzes.Remove(quizToDelete);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<QuizViewModel>> UpdateAsync(QuizEditModel quiz)
        {
            var quizToUpdate = await _db.Quizzes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == quiz.Id);
            if (quizToUpdate == null)
                return Result<QuizViewModel>.NotFound(typeof(Quiz).NotFoundMessage(quiz.Id));

            if (!_identityService.IsAdministrator())
                if (quizToUpdate.CreatedByUserId != _identityService.GetUserId())
                    return Result<QuizViewModel>.Forbiden();

            quizToUpdate.Author = quiz.Author;
            quizToUpdate.Config = quiz.Config;
            quizToUpdate.IsAvalible = quiz.IsAvalible;
            quizToUpdate.From = quiz.From;
            quizToUpdate.To = quiz.To;
            quizToUpdate.IsTemplate = quiz.IsTemplate;
            quizToUpdate.Name = quiz.Name;
            quizToUpdate.PrepareToUpdate(_identityService);
            _db.Quizzes.Update(quizToUpdate);
            await _db.SaveChangesAsync();

            return Result<QuizViewModel>.SuccessWithData(_mapper.Map<QuizViewModel>(quizToUpdate));
        }

        public async Task<Result<bool>> DeleteQuestionAsync(Guid id, int questionId)
        {
            var questionForDelete = await _db.Questions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == questionId);
            if (questionForDelete == null)
                return Result<bool>.NotFound(typeof(Question).NotFoundMessage(questionId));

            if (questionForDelete.QuizId != id)
                return Result<bool>.Forbiden();

            if (!_identityService.IsAdministrator())
                if (questionForDelete.CreatedByUserId != _identityService.GetUserId())
                    return Result<bool>.Forbiden();

            _db.Questions.Remove(questionForDelete);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<bool>> DeleteAnswerAsync(Guid id, int questionId, long answerId)
        {
            var answerForDelete = await _db.Answers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == answerId);
            if (answerForDelete == null)
                return Result<bool>.NotFound(typeof(Answer).NotFoundMessage(answerId));

            if (answerForDelete.QuestionId != questionId)
                return Result<bool>.Forbiden();

            if (!_identityService.IsAdministrator())
                if (answerForDelete.CreatedByUserId != _identityService.GetUserId())
                    return Result<bool>.Forbiden();

            _db.Answers.Remove(answerForDelete);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<List<QuestionViewModel>>> UpdateQuestionsAsync(Guid quizId, List<QuestionEditModel> questions)
        {
            var updatedIds = questions.Select(q => q.Id);
            var questionsToUpdate = await _db.Questions
                .AsNoTracking()
                .Where(s => s.QuizId == quizId && updatedIds.Contains(s.Id))
                .ToListAsync();

            if (questionsToUpdate == null || questionsToUpdate.Count == 0)
                return Result<List<QuestionViewModel>>.NotFound($"Questions with IDs ({string.Join(',', updatedIds)}) not found");

            if (questionsToUpdate.Count < updatedIds.Count())
                return Result<List<QuestionViewModel>>.NotFound();

            if (!questionsToUpdate.All(s => s.QuizId == quizId))
                return Result<List<QuestionViewModel>>.Error("Not all questions in current quiz");

            if (!_identityService.IsAdministrator())
                if (!questionsToUpdate.All(s => s.CreatedByUserId == _identityService.GetUserId()))
                    return Result<List<QuestionViewModel>>.Forbiden();

            var diff = updatedIds.Except(questionsToUpdate.Select(s => s.Id));

            if (diff != null || diff.Count() > 0)
                return Result<List<QuestionViewModel>>.Error($"Questions with IDs ({string.Join(',', diff)}) not found");

            foreach (var question in questionsToUpdate)
            {
                var questionViewModel = questions.First(s => s.Id == question.Id);

                question.Index = questionViewModel.Index;
                question.QuestionText = questionViewModel.QuestionText;
                question.IsMultipleAnswers = questionViewModel.IsMultipleAnswers;
                question.PrepareToUpdate(_identityService);
            }

            _db.Questions.UpdateRange(questionsToUpdate);
            await _db.SaveChangesAsync();

            return Result<List<QuestionViewModel>>.SuccessWithData(_mapper.Map<List<QuestionViewModel>>(questionsToUpdate));
        }

        public async Task<Result<List<AnswerViewModel>>> UpdateAnswersAsync(Guid quizId, int questionId, List<AnswerEditModel> answers)
        {
            var updatedIds = answers.Select(q => q.Id);
            var answersToUpdate = await _db.Answers
                .AsNoTracking()
                .Where(s => s.QuestionId == questionId && updatedIds.Contains(s.Id))
                .ToListAsync();

            if (answersToUpdate == null || answersToUpdate.Count == 0)
                return Result<List<AnswerViewModel>>.NotFound($"Answers with IDs ({string.Join(',', updatedIds)}) not found");

            if (answersToUpdate.Count < updatedIds.Count())
                return Result<List<AnswerViewModel>>.NotFound();

            if (!answersToUpdate.All(s => s.QuestionId == questionId))
                return Result<List<AnswerViewModel>>.Error("Not all answers in current quiz");

            if (!_identityService.IsAdministrator())
                if (!answersToUpdate.All(s => s.CreatedByUserId == _identityService.GetUserId()))
                    return Result<List<AnswerViewModel>>.Forbiden();

            var diff = updatedIds.Except(answersToUpdate.Select(s => s.Id));

            if (diff != null || diff.Count() > 0)
                return Result<List<AnswerViewModel>>.Error($"Answers with IDs ({string.Join(',', diff)}) not found");

            foreach (var answer in answersToUpdate)
            {
                var answerViewModel = answers.First(s => s.Id == answer.Id);

                answer.Response = answerViewModel.Response;
                answer.IsCorrect = answerViewModel.IsCorrect;
                answer.PrepareToUpdate(_identityService);
            }

            _db.Answers.UpdateRange(answersToUpdate);
            await _db.SaveChangesAsync();

            return Result<List<AnswerViewModel>>.SuccessWithData(_mapper.Map<List<AnswerViewModel>>(answersToUpdate));
        }

        public async Task<Result<List<QuizViewModel>>> GetAllAsync(int offset = 0, int count = 20)
        {
            var query = _db.Quizzes.AsNoTracking();

            if (!_identityService.IsAdministrator())
                query = query.Where(s => s.CreatedByUserId == _identityService.GetUserId());

            query = query.OrderByDescending(s => s.CreatedAt);
            query = query.Skip(offset).Take(count);
            var quizes = await query.ToListAsync();

            return Result<List<QuizViewModel>>.SuccessWithData(_mapper.Map<List<QuizViewModel>>(quizes));
        }

        public async Task<Result<QuizResultViewModel>> GetResultAsync(Guid quizId, int quizResultId)
        {
            var request = await _commonService.IsExistWithResultsAsync<Quiz>(s => s.Id == quizId);

            if (!request.IsExist)
                return Result<QuizResultViewModel>.NotFound(typeof(Quiz).NotFoundMessage(quizId));

            var quiz = request.Results.First();

            var quizResult = await _db.QuizResults.AsNoTracking().FirstOrDefaultAsync(s => s.Id == quizResultId);

            if (quizResult == null)
                return Result<QuizResultViewModel>.NotFound(typeof(QuizResult).NotFoundMessage(quizResultId));

            if (!_identityService.IsAdministrator())
                if (quizResult.CreatedByUserId != _identityService.GetUserId())
                    if (quiz.CreatedByUserId != _identityService.GetUserId())
                        return Result<QuizResultViewModel>.Forbiden();

            var quizResultViewModel = _mapper.Map<QuizResultViewModel>(quizResult);

            if (!_identityService.IsAdministrator())
                if (quizResult.UserId == _identityService.GetUserId() && !quiz.Config.ShowResults)
                    quizResultViewModel.Result = null;

            return Result<QuizResultViewModel>.SuccessWithData(quizResultViewModel);
        }

        #region Private
        private bool TryMapUserAnswersToQuiz(MapAnswersToQuiz answersToQuiz, out string error)//List<Question> questions, List<QuizAnswerResponse> quizResponse, QuizResult result, bool showCorrectAnswer, out string error)
        {
            if (!TryCheckQuestions(answersToQuiz.Questions, answersToQuiz.QuizResponse, out var errorQuestion))
            {
                error = errorQuestion;
                return false;
            }

            if (!TryCheckAnswers(answersToQuiz.Questions, answersToQuiz.QuizResponse, out var errorAnswers))
            {
                error = errorAnswers;
                return false;
            }


            var maxMark = Math.Round(answersToQuiz.Quiz.Config.MarkPerQuiz, 2);
            var markPerTrueAnswer = maxMark / answersToQuiz.Questions.Count;

            double markResult = 0;

            foreach (var question in answersToQuiz.Questions)
            {
                var resposneModel = answersToQuiz.QuizResponse.FirstOrDefault(s => s.QuestionId == question.Id);

                var questionModel = new QuestionModel
                {
                    Id = question.Id,
                    QuestionText = question.QuestionText,
                    Answers = new List<AnswerModel>()
                };

                var perOneTrueAnswer = markPerTrueAnswer / question.Answers.Count();

                foreach (var answer in question.Answers)
                {
                    var answerModel = new AnswerModel
                    {
                        Id = answer.Id,
                        Response = answer.Response,
                        IsCorrect = answersToQuiz.Quiz.Config.ShowCorrectAnswers ? answer.IsCorrect : null,
                        IsChoice = resposneModel != null ? resposneModel.AnswerIds?.Contains(answer.Id) : null
                    };

                    if (answerModel.IsCorrectAnswer())
                        if (question.IsMultipleAnswers)
                        {
                            markResult += perOneTrueAnswer;
                        }
                        else
                        {
                            markResult += markPerTrueAnswer;
                        }

                    questionModel.Answers.Add(answerModel);
                }
                answersToQuiz.Result.Result.Add(questionModel);
            }
            answersToQuiz.Result.Mark = Math.Round(markResult, 2);


            error = null;
            return true;
        }

        private bool CanViewQuiz(Quiz quiz)
        {
            if (_identityService.IsAdministrator())
                return true;
            if (quiz.CreatedByUserId == _identityService.GetUserId())
                return true;
            if (!quiz.IsAvalible && quiz.CreatedByUserId != _identityService.GetUserId())
                return false;
            return false;
        }

        private bool TryCheckQuestions(List<Question> questions, List<QuizAnswerResponse> quizResponse, out string error)
        {
            var diffQuestions = quizResponse.Select(s => s.QuestionId).Except(questions.Select(s => s.Id));

            if (diffQuestions != null && diffQuestions.Count() > 0)
            {
                if (diffQuestions.Count() == 1)
                    error = $"Питання з Id {diffQuestions.First()} не існує";
                else
                    error = $"Питань з Id ({string.Join(",", diffQuestions)}) не існує";
                return false;
            }
            error = null;
            return true;
        }

        private bool TryCheckAnswers(List<Question> questions, List<QuizAnswerResponse> quizResponse, out string error)
        {
            var originalAnswerdIds = new List<long>();

            questions.ForEach(question =>
            {
                question.Answers.ForEach(answer =>
                {
                    originalAnswerdIds.Add(answer.Id);
                });
            });

            var answerdIds = new List<long>();

            quizResponse.ForEach(response =>
            {
                response.AnswerIds.ToList().ForEach(answer =>
                {
                    answerdIds.Add(answer);
                });
            });

            var diffAnswers = answerdIds.Except(originalAnswerdIds);

            if (diffAnswers != null && diffAnswers.Count() > 0)
            {
                if (diffAnswers.Count() == 1)
                    error = $"Відповіді з Id {diffAnswers.First()} не існує";
                else
                    error = $"Відповідей з Id ({string.Join(",", diffAnswers)}) не існує";
                return false;
            }

            foreach (var response in quizResponse)
            {
                var question = questions.FirstOrDefault(s => s.Id == response.QuestionId);
                var answerDiff = response.AnswerIds.Except(question.Answers.Select(s => s.Id));
                if (answerDiff != null && answerDiff.Count() > 0)
                {
                    error = $"Відповіді з Id ({string.Join(",", answerDiff)}) для питання з Id ({question.Id}) не знайдено";
                    return false;
                }
            }
            error = null;
            return true;
        }

        private async Task<QuizViewModel> ReadyQuizToViewAsync(Quiz quiz)
        {
            var quizViewModel = _mapper.Map<QuizViewModel>(quiz);

            var questions = await _db.Questions.AsNoTracking().Where(s => s.QuizId == quiz.Id).Include(s => s.Answers).ToListAsync();

            if (quiz.Config.RandomQuestionsAndAnswers)
                quizViewModel.Questions = GetRandomQuestions(questions);
            else
                quizViewModel.Questions = GetSortingQuestions(questions);

            return quizViewModel;
        }

        private List<QuestionViewModel> GetSortingQuestions(List<Question> questions)
        {
            var sortingQuestions = new List<QuestionViewModel>();

            if (questions == null || questions.Count == 0)
                return sortingQuestions;

            int questionNumber = 1;
            foreach (var question in questions.OrderBy(s => s.Index))
            {
                var questionViewModel = new QuestionViewModel
                {
                    Id = question.Id,
                    Index = question.Index,
                    CreatedAt = question.CreatedAt,
                    QuestionText = question.QuestionText.GetNumericQuestion(questionNumber),
                    Answers = new List<AnswerViewModel>()
                };
                if (question.Answers != null && question.Answers.Count > 0)
                {
                    int answerNumber = 1;
                    foreach (var answer in question.Answers)
                    {
                        questionViewModel.Answers.Add(new AnswerViewModel
                        {
                            Id = answer.Id,
                            Response = answer.Response.GetNumericAnswer(answerNumber)
                        });
                        answerNumber++;
                    }
                }
                sortingQuestions.Add(questionViewModel);
                questionNumber++;
            }

            return sortingQuestions;
        }

        private List<QuestionViewModel> GetRandomQuestions(List<Question> questions)
        {
            var randomQuestions = new List<QuestionViewModel>();

            if (questions == null || questions.Count == 0)
                return randomQuestions;

            var questionIds = questions.Select(x => x.Id).ToList();

            for (int i = 1; i <= questions.Count; i++)
            {
                var randomQuestionId = questionIds[new Random().Next(0, questionIds.Count)];
                var question = questions.FirstOrDefault(s => s.Id == randomQuestionId);
                var questionViewModel = new QuestionViewModel
                {
                    Id = question.Id,
                    CreatedAt = question.CreatedAt,
                    Index = question.Index,
                    QuestionText = question.QuestionText.GetNumericQuestion(i),
                    Answers = new List<AnswerViewModel>()
                };

                if (question.Answers != null && question.Answers.Count > 0)
                {
                    var answersIds = question.Answers.Select(s => s.Id).ToList();
                    for (int j = 1; j <= question.Answers.Count; j++)
                    {
                        var randomAnswerId = answersIds[new Random().Next(0, answersIds.Count)];
                        var answer = question.Answers.FirstOrDefault(s => s.Id == randomAnswerId);
                        var answerViewModel = new AnswerViewModel
                        {
                            Id = answer.Id,
                            Response = answer.Response.GetNumericAnswer(j),
                        };
                        answersIds.Remove(answer.Id);
                        questionViewModel.Answers.Add(answerViewModel);
                    }
                }
                randomQuestions.Add(questionViewModel);
                questionIds.Remove(question.Id);
            }
            return randomQuestions;
        }
        #endregion
    }
}