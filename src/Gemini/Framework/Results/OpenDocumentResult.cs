using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Framework.Results
{
	public class OpenDocumentResult : OpenResultBase<IScreen>
	{
		private readonly IScreen _editor;
		private readonly Type _editorType;
		private readonly string _path;

		[Import]
		private IShell _shell;

		public OpenDocumentResult(IScreen editor)
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
					? (IScreen) IoC.GetInstance(_editorType, null)
					: GetEditor(_path));

			if (editor == null)
			{
				OnCompleted(null);
				return;
			}

			if (_setData != null)
				_setData(editor);

			if (_onConfigure != null)
				_onConfigure(editor);

			editor.Deactivated += (s, e) =>
			{
				if (_onShutDown != null)
					_onShutDown(editor);

				OnCompleted(null);
			};

			_shell.OpenDocument(editor);
		}

		private static IScreen GetEditor(string path)
		{
			return IoC.GetAllInstances(typeof(IEditorProvider))
				.Cast<IEditorProvider>()
				.Where(provider => provider.Handles(path))
				.Select(provider => provider.Create(path))
				.FirstOrDefault();
		}
	}
}