﻿using Caliburn.Micro;
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

		public static ShowToolResult<TTool> Tool<TTool>()
			where TTool : ITool
		{
			return new ShowToolResult<TTool>();
		}

		public static ShowToolResult<TTool> Tool<TTool>(TTool tool)
			where TTool : ITool
		{
			return new ShowToolResult<TTool>(tool);
		}

		public static OpenDocumentResult Document(IDocument document)
		{
			return new OpenDocumentResult(document);
		}

		public static OpenDocumentResult Document(string path)
		{
			return new OpenDocumentResult(path);
		}

		public static OpenDocumentResult Document<T>()
				where T : IDocument
		{
			return new OpenDocumentResult(typeof(T));
		}

        public static ShowWindowResult<TWindow> Window<TWindow>()
                where TWindow : IWindow
        {
            return new ShowWindowResult<TWindow>();
        }

        public static ShowWindowResult<TWindow> Window<TWindow>(TWindow window)
            where TWindow : IWindow
        {
            return new ShowWindowResult<TWindow>(window);
        }
    }
}