using System;
using Gemini.Framework.Questions;
using Microsoft.Practices.ServiceLocation;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Results
{
	public class MessageBoxResult : IResult
	{
		private readonly string _text;
		private readonly string _caption = "MDI Shell";
		private readonly Action<Answer> _handleResult = delegate { };
		private readonly Answer[] _possibleAnswers = new[] { Answer.Ok };

		public MessageBoxResult(string text)
		{
			_text = text;
		}

		public MessageBoxResult(string text, string caption)
		{
			_text = text;
			_caption = caption;
		}

		public MessageBoxResult(string text, string caption, Action<Answer> handleResult)
		{
			_text = text;
			_caption = caption;
			_handleResult = handleResult;
			_possibleAnswers = new[] { Answer.Cancel, Answer.Ok };
		}

		public MessageBoxResult(string text, string caption, Action<Answer> handleResult, params Answer[] possibleAnswers)
		{
			_text = text;
			_caption = caption;
			_handleResult = handleResult;
			_possibleAnswers = possibleAnswers;
		}

		public string Text
		{
			get { return _text; }
		}

		public string Caption
		{
			get { return _caption; }
		}

		public Answer[] PossibleAnswers
		{
			get { return _possibleAnswers; }
		}

		public void Execute()
		{
			Execute(ServiceLocator.Current);
		}

		public void Execute(IServiceLocator serviceLocator)
		{
			Execute(
					serviceLocator.GetInstance<IWindowManager>(),
					serviceLocator.GetInstance<IQuestionDialog>()
					);
		}

		public void Execute(IWindowManager windowManager, IQuestionDialog questionDialog)
		{
			var question = new Question(
					null,
					Text,
					_possibleAnswers
					);

			questionDialog.Setup(
					Caption,
					new[] { question }
					);

			questionDialog.WasShutdown += delegate
			{
				if (_handleResult != null)
					_handleResult(question.Answer);
				Completed(this, null);
			};

			windowManager.ShowDialog(questionDialog, null, HandleShutdown);
		}

		public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
		{
			Execute();
		}

		private void HandleShutdown(ISubordinate model, Action completed)
		{
			model.Execute(completed);
		}

		public event Action<IResult, Exception> Completed = delegate { };
	}
}