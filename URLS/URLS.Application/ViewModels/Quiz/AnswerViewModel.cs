namespace URLS.Application.ViewModels.Quiz
{
    public class AnswerViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public QuestionViewModel Question { get; set; }
    }
}
