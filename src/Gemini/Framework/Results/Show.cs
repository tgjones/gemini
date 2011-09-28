using Caliburn.Micro;
using Gemini.Framework.Services;
using Microsoft.Win32;

namespace Gemini.Framework.Results
{
	public static class Show
	{
		public static ShowCommonDialogResult Dialog(CommonDialog commonDialog)
		{
			return new ShowCommonDialogResult(commonDialog);
		}

		public static ShowToolResult<TTool> Tool<TTool>(PaneLocation pane)
			where TTool : IScreen
		{
			return new ShowToolResult<TTool>(pane);
		}

		public static ShowToolResult<TTool> Tool<TTool>(PaneLocation pane, TTool tool)
			where TTool : IScreen
		{
			return new ShowToolResult<TTool>(pane, tool);
		}

		public static OpenDocumentResult Document(IScreen document)
		{
			return new OpenDocumentResult(document);
		}

		public static OpenDocumentResult Document(string path)
		{
			return new OpenDocumentResult(path);
		}

		public static OpenDocumentResult Document<T>()
				where T : IScreen
		{
			return new OpenDocumentResult(typeof(T));
		}
	}
}