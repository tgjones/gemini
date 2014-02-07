using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Framework.Results
{
	public class OpenDocumentResult : OpenResultBase<IDocument>
	{
		private readonly IDocument _editor;
		private readonly Type _editorType;
		private readonly string _path;

		[Import]
		private IShell _shell;

		public OpenDocumentResult(IDocument editor)
		{
			_editor = editor;
		}

		public OpenDocumentResult(string path)
		{
			_path = path;
		}

		public OpenDocumentResult(Type editorType)
		{
			_editorType = editorType;
		}

		public override void Execute(ActionExecutionContext context)
		{
			var editor = _editor ??
				(string.IsNullOrEmpty(_path)
					? (IDocument)IoC.GetInstance(_editorType, null)
					: GetEditor(_path));

			if (editor == null)
			{
				OnCompleted(null, true);
				return;
			}

			if (_setData != null)
				_setData(editor);

			if (_onConfigure != null)
				_onConfigure(editor);

			editor.Deactivated += (s, e) =>
			{
				if (!e.WasClosed)
					return;

				if (_onShutDown != null)
					_onShutDown(editor);
			};

			_shell.OpenDocument(editor);

			OnCompleted(null, false);
		}

		private static IDocument GetEditor(string path)
		{
			return IoC.GetAllInstances(typeof(IEditorProvider))
				.Cast<IEditorProvider>()
				.Where(provider => provider.Handles(path))
				.Select(provider => provider.Create(path))
				.FirstOrDefault();
		}
	}
}