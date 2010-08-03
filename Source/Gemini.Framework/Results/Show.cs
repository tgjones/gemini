using System;
using Gemini.Framework.Questions;
using Gemini.Framework.Services;
using Microsoft.Win32;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Results
{
	public static class Show
	{
		public static OpenChildResult<TChild> Child<TChild>()
				where TChild : IExtendedPresenter
		{
			return new OpenChildResult<TChild>();
		}

		public static OpenChildResult<TChild> Child<TChild>(TChild child)
				where TChild : IExtendedPresenter
		{
			return new OpenChildResult<TChild>(child);
		}

		public static OpenModalResult<TModal> Modal<TModal>()
				where TModal : IExtendedPresenter
		{
			return new OpenModalResult<TModal>();
		}

		public static OpenModalResult<TModal> Modal<TModal>(TModal modal)
				where TModal : IExtendedPresenter
		{
			return new OpenModalResult<TModal>(modal);
		}

		public static ShowToolResult<TTool> Tool<TTool>(Pane pane)
				where TTool : IExtendedPresenter
		{
			return new ShowToolResult<TTool>(pane);
		}

		public static ShowToolResult<TTool> Tool<TTool>(Pane pane, TTool tool)
				where TTool : IExtendedPresenter
		{
			return new ShowToolResult<TTool>(pane, tool);
		}

		public static ShowCommonDialogResult Dialog(CommonDialog commonDialog)
		{
			return new ShowCommonDialogResult(commonDialog);
		}

		public static OpenDocumentResult Document(IExtendedPresenter document)
		{
			return new OpenDocumentResult(document);
		}

		public static OpenDocumentResult Document(string path)
		{
			return new OpenDocumentResult(path);
		}

		public static OpenDocumentResult Document<T>()
				where T : IExtendedPresenter
		{
			return new OpenDocumentResult(typeof(T));
		}

		public static MessageBoxResult MessageBox(string text)
		{
			return new MessageBoxResult(text);
		}

		public static MessageBoxResult MessageBox(string text, string caption)
		{
			return new MessageBoxResult(text, caption);
		}

		public static MessageBoxResult MessageBox(string text, string caption, Action<Answer> handleResult)
		{
			return new MessageBoxResult(text, caption, handleResult);
		}

		public static MessageBoxResult MessageBox(string text, string caption, Action<Answer> handleResult, params Answer[] possibleAnswers)
		{
			return new MessageBoxResult(text, caption, handleResult, possibleAnswers);
		}
	}
}