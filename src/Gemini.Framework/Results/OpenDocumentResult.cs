using System;
using Gemini.Framework.Services;
using Microsoft.Practices.ServiceLocation;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Results
{
	public class OpenDocumentResult : OpenResultBase<IExtendedPresenter>
	{
		private readonly IExtendedPresenter _editor;
		private readonly Type _editorType;
		private readonly string _path;

		public OpenDocumentResult(IExtendedPresenter editor)
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

		public override void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
		{
			var shell = ServiceLocator.Current.GetInstance<IShell>();
			var editor = _editor ??
				(string.IsNullOrEmpty(_path)
											 ? (IExtendedPresenter) ServiceLocator.Current.GetInstance(_editorType)
											 : ServiceLocator.Current.GetEditor(_path));

			if (editor == null)
			{
				OnCompleted(null);
				return;
			}

			if (_setData != null)
				_setData(editor);

			if (_onConfigure != null)
				_onConfigure(editor);

			editor.WasShutdown +=
					(s, e) =>
					{
						if (_onShutDown != null)
							_onShutDown(editor);

						OnCompleted(null);
					};

			shell.OpenDocument(editor);
		}
	}
}