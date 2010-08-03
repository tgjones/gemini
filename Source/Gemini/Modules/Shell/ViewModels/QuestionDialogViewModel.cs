using System.Collections.Generic;
using Caliburn.PresentationFramework;
using Gemini.Framework;
using Gemini.Framework.Questions;

namespace Gemini.Modules.Shell.ViewModels
{
	public class QuestionDialogViewModel : Screen, IQuestionDialog
    {
        private string _caption = "MDI Shell";

        public bool HasOneQuestion
        {
            get { return Questions.Count == 1; }
        }

        public bool HasMultipleQuestions
        {
            get { return Questions.Count > 1; }
        }

        public Question FirstQuestion
        {
            get { return Questions[0]; }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                NotifyOfPropertyChange("Caption");
            }
        }

        public IObservableCollection<Question> Questions { get; private set; }

        public void Setup(string caption, IEnumerable<Question> questions)
        {
            Caption = caption;
            Questions = new BindableCollection<Question>(questions);
        }

        public void SelectAnswer(Answer answer)
        {
            FirstQuestion.Answer = answer;
            Close();
        }
    }
}