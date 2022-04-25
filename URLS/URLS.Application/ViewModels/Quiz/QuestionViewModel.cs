namespace URLS.Application.ViewModels.Quiz
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public QuizViewModel Quiz { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
    }
}
